using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace DesktopSorter.FileManager
{
    public class FileMover : IFileMover
    {
        public string DestinationPath { set; get; }
        public FileMover(string destinationPath)
        {
            this.DestinationPath = destinationPath;
        }
        public FileMover()
        {
            this.DestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + App.Current.MainWindow.Title;
        }
        /// <summary>
        /// Перемещает указаныый файл в DestinationPath
        /// </summary>
        /// <param name="fileToMove">Путь перемещаемого файла</param>
        /// <exception cref="IOException"/>
        /// <exception cref="FileNotFoundException"/>
        public void MoveFile(string fileToMove)
        {
            var directory = new FileInfo(fileToMove);
            if (!directory.Attributes.HasFlag(FileAttributes.Directory))
            {
                try
                {
                    var destFilePath = DestinationPath + GetShortFileName(fileToMove);
                    if (File.Exists(destFilePath))
                    {
                        var exception = new IOException("File exist in destination folder", 333);
                        exception.Data.Add("file", fileToMove);
                        throw exception;
                    }
                    File.Move(fileToMove, destFilePath);
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Error move file:\n\t{error.Message}");
                    throw;
                }
            } else
            {
                throw new FileNotFoundException("This path isn`t file", directory.FullName);
            }
        }

        public void MoveFiles(IEnumerable<string> filesToMove)
        {
            var filesEnumerator = filesToMove.GetEnumerator();
            try
            {
                while (filesEnumerator.MoveNext())
                {
                    MoveFile(filesEnumerator.Current);
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
            try
            {
                var fileDestPath = DestinationPath + destinationFileName;
                if (File.Exists(fileDestPath))
                {
                    var exception =  new IOException($"File {fileToCopy}\n is exist in destination folder", 333);
                    exception.Data.Add("file", fileToCopy);
                    throw exception;
                }
                File.Copy(fileToCopy, fileDestPath);
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error Copy file:\n\t{error.Message}");
                throw;
            }
            
        }

        public void DeleteFile(string deletingFile)
        {
            try
            {
                File.Delete(deletingFile);
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error delete file:\n\t{error.Message}");
                throw;
            }
        }

        private string GetShortFileName(string fileName)
        {
            var lastSlashIndex = fileName.LastIndexOf('\\');
            if (lastSlashIndex < 0)
                lastSlashIndex = fileName.LastIndexOf('/');
            return fileName.Substring(lastSlashIndex);
        }
    }
}
