using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pckkey.Core.ArchiveEngine
{
    public class ArchiveKey
    {
        public string Name { get; set; }
        public int KEY_1 { get; set; }
        public int KEY_2 { get; set; }
        public int ASIG_1 { get; set; }
        public int ASIG_2 { get; set; }
        public int FSIG_1 { get; set; }
        public int FSIG_2 { get; set; }

        public ArchiveKey()
        {

        }
    }
}
