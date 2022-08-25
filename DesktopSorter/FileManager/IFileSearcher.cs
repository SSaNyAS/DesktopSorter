using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopSorter.FileManager
{
    public interface IFileSearcher
    {
        public string SearchRoot { get; set; }
        IEnumerable<string> GetFiles(string filter);
    }
}
