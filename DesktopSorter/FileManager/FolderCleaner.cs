using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSorter.FileManager
{
    public class FolderCleaner : IDisposable
    {
        public FolderCleaner(string searchFolder, string filter, string destinationFolder)
        {
            this.FileSearcher = new FileSearcher();
            var fileMoverDefault = new FileMover();

            var fileNameUnduplicateCorrector = new FileNameCorrector(new FileSearcher());

            this.FileMover = new FileMoverUnduplicated(fileMoverDefault, fileNameUnduplicateCorrector);

            this.SystemWatcher = new FileSystemWatcher();

            this.SearchFolder = searchFolder;
            this.DestinationFolder = destinationFolder;
            this.Filter = filter;

            this.SystemWatcher.Created += SystemWatcher_FileCreated;
            this.SystemWatcher.Changed += SystemWatcher_FileChanged;
            this.SystemWatcher.Renamed += SystemWatcher_FileRenamed;
            this.SystemWatcher.Deleted += SystemWatcher_FileDeleted;
            this.SystemWatcher.Error += SystemWatcher_Error;
        }
        public string DestinationFolder
        {
            get
            {
                return FileMover.DestinationPath;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    FileMover.DestinationPath = value;
                }
            }
        }
        public string SearchFolder
        {
            get
            {
                return FileSearcher.SearchRoot;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    FileSearcher.SearchRoot = value;
                    this.SystemWatcher.Path = FileSearcher.SearchRoot;
                }
            }
        }
        public string Filter
        {
            get
            {
                return this.SystemWatcher.Filter;
            }
            set
            {
                this.SystemWatcher.Filter = value;
            }
        }
        IFileSearcher FileSearcher { get; set; }
        IFileMover FileMover { get; set; }
        FileSystemWatcher SystemWatcher { get; set; }
        #region Events

        public event CreateFileEventHandler FileCreated;
        public delegate void CreateFileEventHandler(object sender, string filePath);

        public event ChangeFileEventHandler FileChanged;
        public delegate void ChangeFileEventHandler(object sender, string filePath);

        public event RenameFileEventHandler FileRenamed;
        public delegate void RenameFileEventHandler(object sender, RenamedEventArgs e);

        public event DeleteFileEventHandler FileDeleted;
        public delegate void DeleteFileEventHandler(object sender,string filePath);

        public event ErrorEventHandler Error;
        public delegate void ErrorEventHandler(object sender, UnknownErrorEventArgs e);
        #endregion


        // Test naming
        public class UnknownErrorEventArgs: EventArgs
        {
            public string Filter { get; private set; }
            public string FolderPath { get; private set; }
            public Exception InnerException { get; private set; }
            public UnknownErrorEventArgs(string folderPath, Exception innerException = null, string filter = "")
            {
                this.FolderPath = folderPath;
                this.Filter = filter;
                this.InnerException = innerException;
            }
        }

        private void SystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            var errorArgs = new UnknownErrorEventArgs(this.SearchFolder, e.GetException(), this.Filter);
            Error?.Invoke(this, errorArgs);
        }

        public void StartWatchingDirectory() => this.SystemWatcher.EnableRaisingEvents = true;
        public void StopWatchingDirectory() => this.SystemWatcher.EnableRaisingEvents = false;
        private void SystemWatcher_FileDeleted(object sender, FileSystemEventArgs e)
        {
            FileDeleted?.Invoke(this, e.FullPath);
        }

        private void SystemWatcher_FileRenamed(object sender, RenamedEventArgs e)
        {
            FileRenamed?.Invoke(this, e);
        }

        private void SystemWatcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            FileChanged?.Invoke(this, e.FullPath);
        }

        private void SystemWatcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            var fullPath = e.FullPath;
            FileMover.MoveFile(fullPath);
            FileCreated?.Invoke(this, fullPath);
        }

        public async Task ScanFolderAndMove()
        {
            var fileMover = this.FileMover;

            var findFilesTask = FindFiles();
            var findedFiles = await findFilesTask;

            var moveFilesTask = new Task(() => {
                fileMover.MoveFiles(findedFiles);
            });
            await moveFilesTask;
        }
        public async Task<IEnumerable<string>> FindFiles()
        {
            var fileSearch = this.FileSearcher;
            var filter = this.Filter;
            var result = await Task<IEnumerable<string>>.Run(() => {
                return fileSearch.GetFiles(filter);
            });
            return result;
        }

        public void Dispose()
        {
            this.SystemWatcher.Dispose();
        }
    }
}
