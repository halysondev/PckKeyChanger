using pckkey.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pckkey.Core.ArchiveEngine
{
    public class ArchiveEntryV2 : IArchiveEntry
    {
        public string Path { get; set; }
        public long Offset { get; set; }
        public int Size { get; set; }
        public int CSize { get; set; }

        public ArchiveEntryV2()
        {

        }

        public ArchiveEntryV2(byte[] data)
        {
            Read(data);
        }

        public void Read(byte[] data)
        {
            if (data.Length < 276)
                data = Zlib.Decompress(data, 276);
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            Path = br.ReadBytes(260).ToGBK().Replace("/", "\\");
            Offset = br.ReadUInt32();
            Size = br.ReadInt32();
            CSize = br.ReadInt32();
            br.Close();
        }

        public byte[] Write(int cl)
        {
            MemoryStream msb = new MemoryStream(new byte[276]);
            BinaryWriter bw = new BinaryWriter(msb);
            bw.Write(Path.Replace("/", "\\").FromGBK());
            bw.BaseStream.Seek(260, SeekOrigin.Begin);
            bw.Write((uint)Offset);
            bw.Write(Size);
            bw.Write(CSize);
            bw.Write(0);
            bw.BaseStream.Seek(0, SeekOrigin.Begin);
            bw.Close();
            byte[] compressed = Zlib.Compress(msb.ToArray(), cl);
            return compressed.Length < 276 ? compressed : msb.ToArray();
        }
    }
}
