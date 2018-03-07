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
// Copyright (c) 2016 William Schubarg
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace LottoNumbersService
{
    #region prevent screensaver, display dimming and automatically sleeping
    public static class OS_CpuBehavior
    {
        static POWER_REQUEST_CONTEXT _PowerRequestContext;
        static IntPtr _PowerRequest; //HANDLE

        // Availability Request Functions
        [DllImport("kernel32.dll")]
        static extern IntPtr PowerCreateRequest(ref POWER_REQUEST_CONTEXT Context);

        [DllImport("kernel32.dll")]
        static extern bool PowerSetRequest(IntPtr PowerRequestHandle, PowerRequestType RequestType);

        [DllImport("kernel32.dll")]
        static extern bool PowerClearRequest(IntPtr PowerRequestHandle, PowerRequestType RequestType);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int CloseHandle(IntPtr hObject);

        // Availablity Request Enumerations and Constants
        enum PowerRequestType
        {
            PowerRequestDisplayRequired = 0,
            PowerRequestSystemRequired,
            PowerRequestAwayModeRequired,
            PowerRequestMaximum
        }

        const int POWER_REQUEST_CONTEXT_VERSION = 0;
        const int POWER_REQUEST_CONTEXT_SIMPLE_STRING = 0x1;
        const int POWER_REQUEST_CONTEXT_DETAILED_STRING = 0x2;

        // Availablity Request Structures
        // Note:  Windows defines the POWER_REQUEST_CONTEXT structure with an
        // internal union of SimpleReasonString and Detailed information.
        // To avoid runtime interop issues, this version of 
        // POWER_REQUEST_CONTEXT only supports SimpleReasonString.  
        // To use the detailed information,
        // define the PowerCreateRequest function with the first 
        // parameter of type POWER_REQUEST_CONTEXT_DETAILED.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct POWER_REQUEST_CONTEXT
        {
            public UInt32 Version;
            public UInt32 Flags;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string
                SimpleReasonString;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PowerRequestContextDetailedInformation
        {
            public IntPtr LocalizedReasonModule;
            public UInt32 LocalizedReasonId;
            public UInt32 ReasonStringCount;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string[] ReasonStrings;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct POWER_REQUEST_CONTEXT_DETAILED
        {
            public UInt32 Version;
            public UInt32 Flags;
            public PowerRequestContextDetailedInformation DetailedInformation;
        }

        /// <summary>
        /// Prevent screensaver, display dimming and power saving. This function wraps PInvokes on Win32 API. 
        /// </summary>
        /// <param name="enableConstantDisplayAndPower">True to get a constant display and power - False to clear the settings</param>
        public static void EnableConstantDisplayAndPower(bool enableConstantDisplayAndPower)
        {
            if (enableConstantDisplayAndPower)
            {
                // Set up the diagnostic string
                _PowerRequestContext.Version = POWER_REQUEST_CONTEXT_VERSION;
                _PowerRequestContext.Flags = POWER_REQUEST_CONTEXT_SIMPLE_STRING;
                _PowerRequestContext.SimpleReasonString = "Continuous measurement"; // your reason for changing the power settings;

                // Create the request, get a handle
                _PowerRequest = PowerCreateRequest(ref _PowerRequestContext);

                // Set the request
                PowerSetRequest(_PowerRequest, PowerRequestType.PowerRequestSystemRequired);
                PowerSetRequest(_PowerRequest, PowerRequestType.PowerRequestDisplayRequired);
            }
            else
            {
                // Clear the request
                PowerClearRequest(_PowerRequest, PowerRequestType.PowerRequestSystemRequired);
                PowerClearRequest(_PowerRequest, PowerRequestType.PowerRequestDisplayRequired);

                CloseHandle(_PowerRequest);
            }
        }        
    }
    #endregion
    public static class Utility
    {
        internal static bool IsDate(Object obj)
        {
            string strDate = obj.ToString();

            try
            {
                return DateTime.TryParse(strDate, out var dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static bool IsGarbage(string input)
        {                        
            return Regex.IsMatch(input, "[\\~#%&*{}/:<>?|\"]");
        }

        internal static string Sanitize(string input)
        {            
            string replacement = " ";
            Regex regEx = new Regex("[\\~#%&*{}/:<>?|\"-]");

            return Regex.Replace(regEx.Replace(input, replacement), @"\s+", " ");
        }

        internal static int GetLottoUpdateHourMin()
        {
            if (!DateTime.TryParse(ConfigurationManager.AppSettings["LottoUpdateTime"].ToUpper(), out var dt))
            {
                throw new Exception("Failed to parse time in App.Config", new Exception(ConfigurationManager.AppSettings["LottoUpdateTime"]));
            }
            
            return (dt.Hour << 10) + dt.Minute;
        }       

        private static string GetTempPath()
        {
            var target = "";

            try
            {
                target = $"{AppDomain.CurrentDomain.BaseDirectory}Temp";

                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($@"The process failed: {e}");
            }

            // Temp directory.
            return target;
        }

        internal static void LogMessageToFile(string msg)
        {
            // Santity Check
            if (string.IsNullOrEmpty(msg))
                return;

            var sw = File.AppendText($"{GetTempPath()}\\{LottoCommon.CommonCore.LottoNumberServiceLogFile}");

            try
            {
                string logLine = $"{System.DateTime.Now:G}: {msg}.";

                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }        
    }

    public static class ArrayUtilities
    {
        internal static int[] FindAllIndex<T>(this T[] array, Predicate<T> match)
        {
            return array.Select((value, index) => match(value) ? index : -1)
                    .Where(index => index != -1).ToArray();
        }
        // create a subset from a range of indices
        internal static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }

        // create a subset from a specific list of indices
        internal static T[] Subset<T>(this T[] array, params int[] indices)
        {
            T[] subset = new T[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }
            return subset;
        }
    }
}