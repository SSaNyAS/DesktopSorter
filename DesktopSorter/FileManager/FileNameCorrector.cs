using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
namespace DesktopSorter.FileManager
{
    class FileNameCorrector : IFileNameUnduplicateCorrector
    {
        public string DestinationFolder { 
            get => FileSearcher.SearchRoot;
            set => FileSearcher.SearchRoot = value;
        }
        private IFileSearcher FileSearcher { set; get; }

        private string indexRegularExpression = @"\w*\({1}(?<index>\d+)\){1}\.\w*$";

        public FileNameCorrector(IFileSearcher destinationFolderSearcher)
        {
            this.FileSearcher = destinationFolderSearcher;
        }
        public string GetNewNameFor(string file)
        {
            var fileInfo = new FileInfo(file);
            var fileNameWithoutIndex = GetFileNameWithoutExtension(fileInfo.Name);
            var duplicatedFiles = FileSearcher.GetFiles($"{fileNameWithoutIndex}*");
            var filesEnumerator = duplicatedFiles.GetEnumerator();
            int maxIndex = 0;
            while (filesEnumerator.MoveNext())
            {
                var fileIndex = FileIndexParse(filesEnumerator.Current);
                if (fileIndex > maxIndex)
                    maxIndex = fileIndex;
            }
            var newIndex = maxIndex + 1;
            var newNameIndexed = FileNameSetIndex(fileNameWithoutIndex, newIndex);
            var fileExtension = fileInfo.Extension;

            return newNameIndexed + fileExtension;
        }

        private int FileIndexParse(string fileName)
        {
            var regular = new Regex(indexRegularExpression);
            var match = regular.Match(fileName);

            if (match.Groups.TryGetValue("index",out Group indexGroup))
            {
                if (int.TryParse(indexGroup.Value, out int fileNum))
                {
                    return fileNum;
                }
            }
            return 0;
        }
        private string FileNameSetIndex(string fileName, int newIndex)
        {
            var regular = new Regex(indexRegularExpression);
            var match = regular.Match(fileName);

            if (match.Groups.TryGetValue("index", out Group indexGroup))
            {
                    string strCopyHead = fileName.Substring(0, indexGroup.Index);
                    string strCopyEnd = fileName.Substring(indexGroup.Index + indexGroup.Length);
                    
                    string newName = strCopyHead + newIndex.ToString() + strCopyEnd;

                    return newName;
            }
            return fileName + $" ({newIndex})";
        }
        private string GetFileNameWithoutExtension(string fileNameWithExtension)
        {
            var regular = new Regex(indexRegularExpression);
            var match = regular.Match(fileNameWithExtension);
            if (match.Groups.TryGetValue("index", out Group indexGroup))
            {
                string strCopyHead = fileNameWithExtension.Substring(0, indexGroup.Index - 1);
                string newName = strCopyHead.Trim();
                return newName;
            }
            return fileNameWithExtension;
        }

    }
}
