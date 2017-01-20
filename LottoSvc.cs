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
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
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
        protected static bool _bPopulateDatabase;

        // _dtHourMinUpdate to replace the _hourUpdate for increased grandularity.
        private static int _hourMinUpdate;
        //private static int _hourUpdate;        
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
                // Disable sleep mode so windows service will continue to run.
                OS_CpuBehavior.EnableConstantDisplayAndPower(true);

                // Fire once per day @ value in App.config.                            
                //_hourUpdate = Utility.GetLottoUpdateHour();
                
                // Increase Update grandularity
                _hourMinUpdate = Utility.GetLottoUpdateHourMin();

                // Test Connection to SQL Database                
                Database.TestSqlConnectionString();

                Utility.LogMessageToFile("Test Connection worked! " + Database.GetSqlConnectionString());

                // Begin scraping - Timer activated
                // Change function to GetLottos upon release                
                GetLottos();

                _bPopulateDatabase = true;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(lblEventSrc, ex.Message + ": " + ex.InnerException.Message,
                                    EventLogEntryType.Warning, (int)CommonCore.LottoErrorMethod.OnStart);               
            }
        }
       
        protected override void OnStop()
        {
            OS_CpuBehavior.EnableConstantDisplayAndPower(false);
        }
        #endregion

        #region Methods
        
        [DebuggerStepThrough]
        private static void GetLottos()
        {
            // Check to see if we need to populate LottoNumbers table if this is the first time running the 
            // this Window service.
            Timer init_timer = new Timer(58000); // Wait for every 58 seconds so everything settles down.
            init_timer.Elapsed += async (sender, e) => await HandleTimer();
            init_timer.AutoReset = true; // only every interval!
            init_timer.Start();                                    
        }        
        #endregion

        #region Event Handlers
        // Specify what you want to happen when the Elapsed event is raised.
        // When init = true, go and init the database else false, check for
        // update time for the day.
        private static Task HandleTimer()
        {
            try
            {
                var strMsg = string.Empty;
                
                if (_bPopulateDatabase ||
                   ((DateTime.Now.Hour << 10) + DateTime.Now.Minute) == _hourMinUpdate)
                {
                    _bPopulateDatabase = false;

                    strMsg = string.Format("Lotto numbers populated on {0}, Row Count {1}", 
                                             DateTime.Now, 
                                             RetrieveLottoNumbers.LoadLottoEntities());
                }
                // Create a task but do.  For now just a simple log entry...
                Action<object> action = (obj) =>
                {
                    Utility.LogMessageToFile(strMsg);
                };

                Task task = new Task(action, TaskCreationOptions.AttachedToParent);
                task.Start();

                task.Wait();

                return task;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(lblEventSrc, ex.Message + ": " + ex.InnerException.Message,
                                   EventLogEntryType.Warning, (int)CommonCore.LottoErrorMethod.HandleTimer);  

                throw new Exception(ex.Message);
            }            
        }

        #endregion
    }
}
