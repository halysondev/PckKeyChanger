using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static pckkey.Core.Events;

namespace pckkey.Core
{
    public class FSWatcher
    {
        public string Dir { get; set; } = "";
        public string TmpFile { get; set; } = "";

        private Hashtable Watchers = new Hashtable();

        public event FileWatcherCreated FileWatcherCreated;

        public FSWatcher(string path)
        {
            int i = 0;
            FileSystemWatcher watcher;
            foreach (string driveName in Directory.GetLogicalDrives())
            {
                if (Directory.Exists(driveName))
                {
                    watcher = new FileSystemWatcher
                    {
                        Filter = Path.GetFileName(path),
                        NotifyFilter = NotifyFilters.FileName,
                        IncludeSubdirectories = true,
                        Path = driveName
                    };
                    watcher.Created += Created;
                    watcher.EnableRaisingEvents = true;
                    Watchers.Add($"file_watcher{++i}", watcher);
                }
            }
        }

        private void Created(object sender, FileSystemEventArgs e)
        {
            FileWatcherCreated?.Invoke(sender, e);
        }
    }
}
