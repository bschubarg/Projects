using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using LottoAdmin.StateLottoNumbersDataSetTableAdapters;
using Microsoft.Win32;
using LottoCommon;

namespace LottoAdmin
{    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private FileSystemWatcher watcher;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public MainWindow()
        {
            InitializeComponent();          
            
            sldHour.Value = DateTime.Parse(new GetLottoAppSettingsTableAdapter().GetData(0).TriggerFetchColumn.Table.Rows[0].ItemArray[0].ToString()).TimeOfDay.TotalHours;            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Get latest log entries
            var logEntries = new _spGetLatestLottoLogsTableAdapter().GetData((int)CommonCore.LottoAppType.LottoNumbersService);

            foreach (var logEntry in logEntries)
            {
                MyNonStatic.OrderList.Add(new TodoItem() { Title = $"{logEntry.LogEntry} {logEntry.LogEntryDate}", Completion = 0 });
            }

            // Log File Watcher
            // Set up a file watcher for the lotto windows service log  
            try
            {
                LottoServiceLogWatcher(GetLottoServiceLogDir());
            }
            catch (Exception ex)
            {
                // Swallow it
            }

        }
        #region Log File Watcher
        private string GetLottoServiceLogDir()
        {
            // The name of the key must include a valid root.
            var lottoServiceExe = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\LottoNumbersService")?.GetValue("ImagePath").ToString().Trim('"');            
            DirectoryInfo d = new DirectoryInfo(lottoServiceExe ?? throw new InvalidOperationException());
            return d.Parent?.FullName;
        }

        private void LottoServiceLogWatcher(string logFilePath)
        {
            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher
            {
                Path = $"{logFilePath}\\Temp",
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.txt" // Only watch text files.
            };
            // Add event handlers.
            watcher.Changed += OnLogChanged;
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private static void OnLogChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.            
            if (e.ChangeType.Equals(WatcherChangeTypes.Changed))
            {                
                // Read last line of the log and display in listbox
                Application.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    MyNonStatic.SetText($"{File.ReadLines(e.FullPath).Last()}", 0);
                });                
            }
        }
        #endregion
        #region UI Handlers
        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetLottoGameEnable();
        }       

        private void SetLottoGameEnable()
        {
            // Sanity Check
            if (cbLottoState == null ||
                cbLottoGames == null)
                return;            
            
            chkEnable.IsChecked = (int)new LottoStateGameTableAdapter().LottoStateGameExist((int)cbLottoState.SelectedValue, (int)cbLottoGames.SelectedValue) > 0; 
        }

        private void chkEnable_Click(object sender, RoutedEventArgs e)
        {
            new LottoStateGameTableAdapter().InsertLottoStateGame((int)cbLottoState.SelectedValue, (int)cbLottoGames.SelectedValue, 1, chkEnable.IsChecked.Value);

            MyNonStatic.SetText($"{cbLottoState.Text}, {cbLottoGames.Text} = {chkEnable.IsChecked.Value}", chkEnable.IsChecked.Value == false ? 0 : 100);            
        }

        private void sldHour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            lblHour.Content = $"{TimeSpan.FromHours(sldHour.Value):hh\\:mm}" + (TimeSpan.FromHours(sldHour.Value).Hours > 11 ? "PM" : "AM");

        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            new GetLottoAppSettingsTableAdapter().Update(0,
                DateTime.Parse($"{TimeSpan.FromHours(sldHour.Value):hh\\:mm}"));
            
            var message = $"Service to fire at {TimeSpan.FromHours(sldHour.Value):hh\\:mm}";
            
            MyNonStatic.SetText($"{message}", 100);            
        }

        private void btnRunNow_Click(object sender, RoutedEventArgs e)
        {
            WindowsServiceController windowsServiceController = new WindowsServiceController("LottoNumbersService");

            try
            {
                MyNonStatic.SetText($"Restarting Service...", 0);

                windowsServiceController.StartOrRestart();

                MyNonStatic.SetText($"Service Running...", 0);
            }
            catch (Exception ex)
            {
                MyNonStatic.SetText($"Restart Failed...{ex.Message}", 0);                
            }           
        }
        #endregion
    }
    public class TodoItem
    {
        public string Title { get; set; }
        public int Completion { get; set; }
    }
    public class MyNonStatic
    {
        static MyNonStatic()
        {
            OrderList = new ObservableCollection<TodoItem>(); //this is where we init our Static obj
        }

        public static void SetText(string msg, int code)
        {
            OrderList.Insert(0, new TodoItem() { Title = $"{DateTime.Now.ToShortTimeString()}-{msg}", Completion = code });
        }

        public static ObservableCollection<TodoItem> OrderList { set; get; }
    }   
}
