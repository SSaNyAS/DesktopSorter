using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace DesktopSorter.FileManager
{
    public class FileSearcher : IFileSearcher
    {
        public string SearchRoot { set; get; }
        public FileSearcher(string searchRoot)
        {
            this.SearchRoot = searchRoot;
        }
        public FileSearcher()
        {
            this.SearchRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public IEnumerable<string> GetFiles(string filter)
        {
            try
            {
                var findedFiles = Directory.EnumerateFiles(SearchRoot, filter, SearchOption.TopDirectoryOnly);
                return findedFiles;
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error find files:\n\t{error.Message}");
                throw error;
            }
        }
    }
}
