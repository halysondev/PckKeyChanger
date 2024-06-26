using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pckkey.Interfaces
{
    public interface IArchiveEntry
    {
        string Path { get; set; }
        long Offset { get; set; }
        int Size { get; set; }
        int CSize { get; set; }

        void Read(byte[] data);
        byte[] Write(int cl);
    }
}
