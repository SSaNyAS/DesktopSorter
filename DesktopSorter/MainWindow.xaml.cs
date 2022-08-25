using DesktopSorter.FileManager;
using DesktopSorter.FileManager.Commands;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DesktopSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FindFolder { get; set; }
        public string DestinationFolder { get; set; }
        public string Filter { get; set; } = "";
        public List<FolderCleaner> FolderCleaners { get; set; } = new List<FolderCleaner>();
        private BackgroundWorker BackgroundWorker { get; set; }
        private TaskbarIcon Taskbar { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Taskbar = new TaskbarIcon();
            Taskbar.Visibility = WindowState == WindowState.Minimized ? Visibility.Visible : Visibility.Collapsed;
            Taskbar.ToolTipText = "Desktop Sorter";

            var contextMenu = new ContextMenu();
            contextMenu.Background = new SolidColorBrush(Color.FromArgb(255, 255, 20, 20));

            var menuItemCloseApp = CreateMenuItem("Exit", new RelayCommand((_) => {
                Application.Current.Shutdown();
            }));
            var menuItemTest = CreateMenuItem("Test", new RelayCommand((_) => {
                MessageBox.Show("Test command");
            }));

            contextMenu.Items.Add(menuItemTest);
            contextMenu.Items.Add(menuItemCloseApp);

            Taskbar.ContextMenu = contextMenu;

            var weakWindow = new WeakReference<Window>(this);
            Taskbar.LeftClickCommand = new RelayCommand((_) => {
                if (weakWindow.TryGetTarget(out var mainWindow)){ 
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal; 
                }
            });

            FindFolder = @"C:\Users\sshan\Desktop\";
            DestinationFolder = @"C:\Users\sshan\Desktop\png\";
            Filter = "*.png";
            
            DataContext = this;

            var folderCleaner = new FolderCleaner(FindFolder, Filter, DestinationFolder);
            folderCleaner.FileDeleted += FolderCleaner_FileDeleted;
            folderCleaner.StartWatchingDirectory();
            this.FolderCleaners.Add(folderCleaner);
        }

        private void FolderCleaner_FileDeleted(object sender, string filePath)
        {
            var weakRef = new WeakReference<MainWindow>(this);
            listBox1.Dispatcher.Invoke(async () => {
                if (weakRef.TryGetTarget(out var mainWindow)){
                    mainWindow.listBox1.ItemsSource = await mainWindow.FolderCleaners.First()?.FindFiles();
                }
            });
        }

        public void FindClick(object sender, RoutedEventArgs e)
        {
            var weakWindow = new WeakReference<MainWindow>(this);
            listBox1.Dispatcher.Invoke(async () => {
                if (weakWindow.TryGetTarget(out var mainWindow)){
                    var cleaner = mainWindow.FolderCleaners.First();
                    cleaner.Filter = mainWindow.Filter;
                    mainWindow.listBox1.ItemsSource = await cleaner.FindFiles();
                }
            });
        }
        
        public void MoveClick(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem is string @selectedFile)
            {

            }
        }

        private MenuItem CreateMenuItem(string header, ICommand command)
        {
            var menuItem = new MenuItem();
            menuItem.Header = header;
            menuItem.Command = command;
            return menuItem;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
                Taskbar.Visibility = Visibility.Visible;
            } else
            {
                Taskbar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
