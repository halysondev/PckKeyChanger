using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pckkey.Core
{
    public class Events
    {
        public delegate void SetProgress(int val);
        public delegate void SetProgressMax(int val);
        public delegate void SetProgressNext();
        public delegate void LoadData(byte type);
        public delegate void CloseTab(object tab);
        public delegate void FileWatcherCreated(object sender, FileSystemEventArgs e);
    }
}
