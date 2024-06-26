using pckkey.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pckkey.Core.ArchiveEngine
{
    public interface IArchiveManager
    {
        List<IArchiveEntry> Files { get; set; }

        event Events.LoadData LoadData;
        event Events.SetProgress SetProgress;
        event Events.SetProgressMax SetProgressMax;
        event Events.SetProgressNext SetProgressNext;

        void AddFiles(List<string> files, string srcdir, string dstdir);
        void Defrag();
        byte[] GetFile(IArchiveEntry entry, bool reload = true);
        List<byte[]> GetFiles(List<IArchiveEntry> files);
        void ReadFileTable();
        void SaveFileTable(long filetable = -1);

        void SaveFileTableChangedKeys(long filetable = -1, int new_key1 = 0, int new_key2 = 0, int compression_level = 9);

        void ChangeKeys(int new_key1, int new_key2, int compression_level = 9);

        void ReadFileTableChangedKeys(int new_key1 = 0, int new_key2 = 0);
        void UnpackFiles(string srcdir, List<IArchiveEntry> files, string dstdir);
    }
}
