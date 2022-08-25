using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopSorter.FileManager
{
    interface IFileMover
    {
        public string DestinationPath { get; set; }
        void MoveFile(string fileToMove);
        void DeleteFile(string deletingFile);
        void CopyFile(string fileToCopy, string destinationFileName);
        void MoveFiles(IEnumerable<string> filesToMove);
    }
}
