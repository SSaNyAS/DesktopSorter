using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopSorter.FileManager
{
    interface IFileNameUnduplicateCorrector
    {
        string DestinationFolder { get; set; }
        string GetNewNameFor(string file);
    }
}
