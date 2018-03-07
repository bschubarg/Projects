// Lotto Numbers Service and Application.
// Open source.  This is a project to demonstrate 
// the various technologies to use when gathering data
// and publishing data by various means.  I created
// this project only for my personal use.  Any alterations
// by others is welcomed.
// 
// I do not pretend to be an expert on these technologies
// but rather a demonstration of my approach to satisfy
// certain requirements.
// 
// Acknowledgments: https://github.com/rubicon-oss/LicenseHeaderManager/wiki - License Header Snippet
//					https://www.codeproject.com/Articles/1041115/Webscraping-with-Csharp - ScrapySharp
// 
// Modified:  1/8/2017 - Added more grandularity to the timer for Lotto Update Numbers - W.A.S.
//
// Copyright (c) 2016 William Schubarg
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using Timer = System.Timers.Timer;

using LottoCommon;


//  Lotto Windows Service that will run once daily to retrieve lotto numbers from the lottories
//  defined in the Lotto table[s] in the StateLottoNumbers SQL database.
//  The service will populate the necessary tables for later retrieval of the WebAPI service.
//  Make this as lighweight as possible.  Read no entity framework etc...
namespace LottoNumbersService
{    
    public partial class LottoService : ServiceBase
    {
        #region Data Members                              
        private static Timer _retrieveTimer;

        #endregion

        #region LottoService Constructor
        public LottoService()
        {
            InitializeComponent();

            // Event logs are set with a maximum size that determines how many entries 
            // they can contain. When an event log is full, it stops recording new 
            // event information or begins to overwrite earlier entries. If event recording
            // stops, you can use this method to clear the log of existing entries and allow
            // it to start recording events again. 
            // You must have administrator permissions to the computer on which the log resides to clear event log entries.
            // Create an EventLog instance and assign its log name.
            if (!EventLog.SourceExists(lblEventSrc))
                EventLog.CreateEventSource(lblEventSrc, lblEventLog);            
        }
        #endregion

        #region Service Event Handlers
        protected override void OnStart(string[] args)
        {            
            try
            {                
                // Uncomment line below to initiate the debugger
                //Debugger.Launch();

                // Disable sleep mode so windows service will continue to run.
                OS_CpuBehavior.EnableConstantDisplayAndPower(true);
                                
                // Test Connection to SQL Database                
                Database.TestSqlConnectionString();

                Utility.LogMessageToFile("Test Connection worked! " + Database.GetSqlConnectionString());                
                
                // Do we want to run service now?
                if ( args.Length > 0 &&
                    args[0].Equals("RunNow"))
                    RunLottoNumberWorkerThread();
                
                // Run service when the LottoApp setting says so
                SetTimer();               
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(lblEventSrc, ex.Message + ": " + ex.InnerException?.Message,
                                    EventLogEntryType.Warning, (int)CommonCore.LottoErrorMethod.OnStart);               
            }
        }       
        protected override void OnStop()
        {
            OS_CpuBehavior.EnableConstantDisplayAndPower(false);

            //Dispose Timer Resource
            _retrieveTimer.Stop();
            _retrieveTimer.Dispose();

        }        
        #endregion

        #region Methods

        #endregion

        #region Event Handlers
        // Specify what you want to happen when the Elapsed event is raised.        
        private static void HandleLottoTask(Object source, ElapsedEventArgs e)
        {
            try
            {
                // Get trigger date.  Do this here since the trigger time can change via LottoAdmin
                // and this will appear more seamless instead of restarting the service.
                var triggerTime = new DateTime();
                var LottoAppSettings = new List<LottoAppSetting>();

                var paramsList = new List<Tuple<int, string, int>>
                {
                    new Tuple<int, string, int>((int)SqlDbType.Int, "LottoApp", 0)
                };

                if (Database.LoadEntities(CommonCore.spGetTriggerTime, ref LottoAppSettings, paramsList) > 0)
                    triggerTime = LottoAppSettings[0].TriggerFetch;

                if (!DateTime.Now.Hour.Equals(triggerTime.Hour) ||
                    !DateTime.Now.Minute.Equals(triggerTime.Minute)) return;

                RunLottoNumberWorkerThread();                
            }
            catch (Exception ex)
            {
                Utility.LogMessageToFile(ex.Message + ": " + ex.InnerException?.Message);

                EventLog.WriteEntry(lblEventSrc, ex.Message + ": " + ex.InnerException?.Message,
                                   EventLogEntryType.Warning, (int)CommonCore.LottoErrorMethod.HandleTimer);  

                throw new Exception(ex.Message);
            }            
        }

        private static void RunLottoNumberWorkerThread()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            bw.RunWorkerAsync(Assembly.GetExecutingAssembly().Location);
        }

        private static void SetTimer()
        {
            // Create a timer with a 58 second interval.
            _retrieveTimer = new Timer(58000);
            
            // Hook up the Elapsed event for the timer. 
            _retrieveTimer.Elapsed += HandleLottoTask;
            _retrieveTimer.AutoReset = true;
            _retrieveTimer.Enabled = true;
        }        
        #endregion
        #region bw_DoWork

        private static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {                
                new RetrieveLottoNumbers().LoadLottoEntities();
            }
            catch (Exception exception)
            {
                e.Cancel = true;                               
            }
        }

        #endregion // bw_DoWork

        #region bw_RunWorkerCompleted

        private static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {                
                // Function will check for Empty or Null message strings
                Utility.LogMessageToFile($"Lotto numbers populated on {DateTime.Now}");                
            }
        }

        #endregion // bw_RunWorkerCompleted
    }
}
