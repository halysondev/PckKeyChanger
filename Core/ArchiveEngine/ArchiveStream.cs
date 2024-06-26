using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pckkey.Core.ArchiveEngine
{
    public class ArchiveStream : IDisposable
    {
        protected BufferedStream pck = null;
        protected BufferedStream pkx = null;
        protected BufferedStream pkx1 = null;
        private string path = "";
        public long Position = 0;
        const uint PCK_MAX_SIZE = 2147483392;
        const uint PKX_MAX_SIZE = 4294966784;
        const int BUFFER_SIZE = 16777216; // 33554432

        public ArchiveStream(string path)
        {
            this.path = path;
        }

        public void Reopen(bool ro)
        {
            Close();
            pck = OpenStream(path, ro);
            if (File.Exists(path.Replace(".pck", ".pkx")) && Path.GetExtension(path) != ".cup")
            {
                pkx = OpenStream(path.Replace(".pck", ".pkx"), ro);

                if (File.Exists(path.Replace(".pck", ".pkx1")) && Path.GetExtension(path) != ".cup")
                {
                    pkx1 = OpenStream(path.Replace(".pck", ".pkx1"), ro);
                }
            }
        }

        private BufferedStream OpenStream(string path, bool ro = true)
        {
            FileAccess fa = ro ? FileAccess.Read : FileAccess.ReadWrite;
            FileShare fs = ro ? FileShare.Read : FileShare.ReadWrite;
            return new BufferedStream(new FileStream(path, FileMode.OpenOrCreate, fa, fs), BUFFER_SIZE);
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            long max_size = 2147483392L + 4294966784L;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = GetLenght() + offset;
                    break;
            }
            if (Position <= pck.Length)
            {
                pck.Seek(Position, SeekOrigin.Begin);
            }
            else if (Position > pck.Length && Position <= pck.Length + pkx.Length)
            {
                pkx.Seek(Position - pck.Length, SeekOrigin.Begin);
            }
            else
            {
                pkx1.Seek(Position - pck.Length - pkx.Length, SeekOrigin.Begin);
            }

        }

        public long GetLenght() => pkx1 != null ? pck.Length + pkx.Length + pkx1.Length : pkx != null ? pck.Length + pkx.Length : pck.Length;

        public void Cut(long len)
        {
            if (len < PCK_MAX_SIZE)
            {
                pck.SetLength(len);
            }
            else if (len < (long)PCK_MAX_SIZE + PKX_MAX_SIZE)
            {
                pkx.SetLength(PCK_MAX_SIZE - len);
            }
            else
            {
                pkx1.SetLength((long)PCK_MAX_SIZE + PKX_MAX_SIZE - len);
            }
        }

        public byte[] ReadBytes(int count)
        {
            byte[] array = new byte[count];
            int BytesRead = 0;
            if (Position < pck.Length)
            {
                BytesRead = pck.Read(array, 0, count);
                if (BytesRead < count && pkx != null)
                {
                    pkx.Seek(0, SeekOrigin.Begin);
                    BytesRead += pkx.Read(array, BytesRead, count - BytesRead);
                }
                if (BytesRead < count && pkx1 != null)
                {
                    pkx1.Seek(0, SeekOrigin.Begin);
                    BytesRead += pkx1.Read(array, BytesRead, count - BytesRead);
                }
            }
            else if (Position < pck.Length + pkx.Length)
            {
                BytesRead = pkx.Read(array, 0, count);
                if (BytesRead < count && pkx1 != null)
                {
                    pkx1.Seek(0, SeekOrigin.Begin);
                    BytesRead += pkx1.Read(array, BytesRead, count - BytesRead);
                }
            }
            else if (pkx1 != null)
            {
                BytesRead = pkx1.Read(array, 0, count);
            }
            Position += count;
            return array;
        }

        public void WriteBytes(byte[] array)
        {
            long totalSize = (long)PCK_MAX_SIZE + PKX_MAX_SIZE;
            long positionAfterWrite = Position + array.Length;

            if (positionAfterWrite < PCK_MAX_SIZE)
            {
                pck.Write(array, 0, array.Length);
            }
            else if (positionAfterWrite < totalSize)
            {
                if (pkx == null)
                {
                    pkx = OpenStream(path.Replace(".pck", ".pkx"), false);
                }
                if (Position >= PCK_MAX_SIZE)
                {
                    pkx.Write(array, 0, array.Length);
                }
                else
                {
                    int pckWriteLength = (int)(PCK_MAX_SIZE - Position);
                    pck.Write(array, 0, pckWriteLength);
                    pkx.Write(array, pckWriteLength, array.Length - pckWriteLength);
                }
            }
            else
            {
                if (pkx1 == null)
                {
                    pkx1 = OpenStream(path.Replace(".pck", ".pkx1"), false);
                }
                if (Position >= totalSize)
                {
                    pkx1.Write(array, 0, array.Length);
                }
                else
                {
                    if (pkx == null)
                    {
                        pkx = OpenStream(path.Replace(".pck", ".pkx"), false);
                    }
                    long pkxPositionStart = PCK_MAX_SIZE;
                    long pkx1PositionStart = totalSize;
                    int pkxBytes = (int)(pkx1PositionStart - Position);

                    if (Position < pkxPositionStart)
                    {
                        int pckWriteLength = (int)(pkxPositionStart - Position);
                        pck.Write(array, 0, pckWriteLength);
                        pkx.Write(array, pckWriteLength, pkxBytes - pckWriteLength);
                        pkx1.Write(array, pkxBytes, array.Length - pkxBytes);
                    }
                    else
                    {
                        pkx.Write(array, 0, pkxBytes);
                        pkx1.Write(array, pkxBytes, array.Length - pkxBytes);
                    }
                }
            }
            Position += array.Length;
        }


        public short ReadInt16() => BitConverter.ToInt16(ReadBytes(2), 0);
        public ushort ReadUInt16() => BitConverter.ToUInt16(ReadBytes(2), 0);
        public int ReadInt32() => BitConverter.ToInt32(ReadBytes(4), 0);
        public uint ReadUInt32() => BitConverter.ToUInt32(ReadBytes(4), 0);
        public long ReadInt64() => BitConverter.ToInt64(ReadBytes(8), 0);
        public ulong ReadUInt64() => BitConverter.ToUInt64(ReadBytes(8), 0);

        public void WriteInt16(short value) => WriteBytes(BitConverter.GetBytes(value));
        public void WriteUInt16(ushort value) => WriteBytes(BitConverter.GetBytes(value));
        public void WriteInt32(int value) => WriteBytes(BitConverter.GetBytes(value));
        public void WriteUInt32(uint value) => WriteBytes(BitConverter.GetBytes(value));
        public void WriteInt64(long value) => WriteBytes(BitConverter.GetBytes(value));
        public void WriteUInt64(ulong value) => WriteBytes(BitConverter.GetBytes(value));

        public void Dispose()
        {
            pck?.Close();
            pkx?.Close();
            pkx1?.Close();
        }

        public void Close()
        {
            pck?.Close();
            pkx?.Close();
            pkx1?.Close();
        }
    }
}
