using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DesktopSorter.FileManager
{
    class FileMoverUnduplicated : IFileMover
    {
        public bool GenerateNewNameIfExist { get; set; } = true;
        private IFileMover fileMover;
        public IFileNameUnduplicateCorrector UnduplicateCorrector { get; set; }
        public string DestinationPath
        {
            get => fileMover.DestinationPath;
            set
            {
                fileMover.DestinationPath = value;
                UnduplicateCorrector.DestinationFolder = fileMover.DestinationPath;
            }
        }
        
        public FileMoverUnduplicated(IFileMover fileMover, IFileNameUnduplicateCorrector fileNameUnduplicateCorrector)
        {
            this.fileMover = fileMover;
            this.UnduplicateCorrector = fileNameUnduplicateCorrector;
        }

        public void MoveFile(string fileToMove)
        {
            try
            {
                fileMover.MoveFile(fileToMove);
            }
            catch (IOException ioException)
            {
                if (ioException.HResult == 333 && GenerateNewNameIfExist)
                {
                    var generatedName = UnduplicateCorrector.GetNewNameFor(fileToMove);
                    CopyFile(fileToMove, generatedName);
                    DeleteFile(fileToMove);
                    return;
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void MoveFiles(IEnumerable<string> filesToMove)
        {
            try
            {
                var enumerator = filesToMove.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MoveFile(enumerator.Current);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error moving files:\n\t{error.Message}");
                throw;
            }
        }

        public void CopyFile(string fileToCopy, string destinationFileName)
        {
            fileMover.CopyFile(fileToCopy, destinationFileName);
        }

        public void DeleteFile(string deletingFile)
        {
            fileMover.DeleteFile(deletingFile);
        }
    }
}
