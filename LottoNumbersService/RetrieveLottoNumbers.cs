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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using LottoCommon;
using ScrapySharp.Network;

namespace LottoNumbersService
{
    public class RetrieveLottoNumbers
    {
        private static readonly LottoNumbers LottoNumbers = new LottoNumbers();
        private static readonly LottoEntities LottoList = new LottoEntities();

        public RetrieveLottoNumbers()
        {
            try
            {
                var Lottos = new List<Lotto>();
                Database.LoadEntities(CommonCore.spGetLottos, ref Lottos);

                EventLogEntry($"Retrieving Lotto Numbers.");

                // Package LottoEntity
                Lottos.ForEach(AddLotto);

                EventLogEntry($"Retrieving Completed.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void AddLotto(Lotto obj)
        {
            LottoList.AddLotto(obj);
        }
        internal void LoadLottoEntities()
        {
            try
            {
                LottoList.ForEach(ScrapeLottoPage);                
            }            
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ": " + ex.InnerException?.Message);
            }
        }
        private void ScrapeLottoPage(LottoEntity entity)
        {
            var lottoYear = DateTime.Now.Year;
            // Sanity check
            while (lottoYear > 1989)
            {
                // Keep Parsing until page not found
                foreach (var month in CommonCore.MonthStrings)
                    try
                    {
                        if (!SkipLottoFetch(entity, month, lottoYear))
                        {
                            ParseLottoHtml(entity, GetPageInnerHtml(entity, month, lottoYear));
                            int count = InsertEntityNumbers(entity, month, lottoYear);

                            EventLogEntry($"{entity.State}: {count} numbers added");
                            Utility.LogMessageToFile($"{entity.State}: {count} numbers added");
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        // If the year is the same is now, then we are looking at future pages that 
                        // do not exist yet.  Continue until a page does not exist in the past.
                        if (lottoYear == DateTime.Now.Year)
                            continue;

                        return;
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"Scrape Lotto Page {entity.LottoUrl} failed. {exception.Message}");
                    }

                lottoYear--;
            }
        }
        /// <summary>
        /// If the result is false for any game, assume it is a new game added or new data.
        /// Also check to see if the date is in the future.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="month"></param>
        /// <param name="lottoYear"></param>
        /// <returns></returns>
        private static bool SkipLottoFetch(LottoEntity entity, string month, int lottoYear)
        {
            int nMonthNormalized = 12 - Array.IndexOf(CommonCore.MonthStrings, month);
            var dtLottoDate = new DateTime(lottoYear, nMonthNormalized, 1);

            // In the future?
            if (dtLottoDate > DateTime.Now) return true;
            
            // or this month? - Always update lotto numbers for present month since the same page is updated
            if (dtLottoDate.Month.Equals(DateTime.Now.Month)) return false;

            foreach (var entityLottoGame in entity.LottoGames)
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@StateID", entity.StateID),
                    new SqlParameter("@GameID", entityLottoGame.LottoGameID),
                    new SqlParameter("@LottoDate", dtLottoDate)
                };

                if (Database.ExecuteScalar(CommonCore.spIsLottoGameFetched, parameters.ToArray()).Rows[0].ItemArray[0].Equals(0))
                    return false;                
            }

            return true;
        }
        private static int InsertEntityNumbers(LottoEntity entity, string month, int lottoYear)
        {
            var lstLottoFetch = new List<LottoFetch>();

            foreach (var entityLottoGame in entity.LottoGames)
            {
                lstLottoFetch.Add(new LottoFetch() {GameID = entityLottoGame.LottoGameID,
                                                    StateID = entity.StateID,
                                                    LottoDate = new DateTime(lottoYear, 12 - Array.IndexOf(CommonCore.MonthStrings, month), 1)

                });                
            }
            // Notify database that we fetched numbers for this state, game and date.
            Database.InsertEntities<LottoFetch>(CommonCore.tblLottoFetched, lstLottoFetch.ConvertToDataTable());
            // Insert into database the numbers
            Database.InsertEntities<LottoNumber>(CommonCore.tblLottoNumber, LottoNumbers.ConvertToDataTable());
            
            // Get number of numbers....
            int nCount = LottoNumbers.Count;
            // Clear the data member Lotto Number list
            LottoNumbers.Clear();

            return nCount;
        }
        /// <summary>
        /// Load page innerHtml into a key paired list.  Each page will have more than one GameID.  Read a single page
        /// for the multiple games for a given month & year for a given State[entity].
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        private static string GetPageInnerHtml(LottoEntity entity, string month, int year)
        {
            // Third party shareware DLL to get web page text.  Why reinvent the wheel?   
            // Scrapy Sharp is an open source scrape framework that combines a web client
            // able to simulate a web browser, and an HtmlAgilityPack extension to select 
            // elements using css selector (like JQuery). Scrapysharp greatly reduces the 
            // workload, upfront pain and setup normally involved in scraping a web-page.
            // https://www.codeproject.com/Articles/1041115/Webscraping-with-Csharp

            // Does the page innerHtml exist in our cache?
            ObjectCache cache = MemoryCache.Default;

            if (cache[$"{entity.StateID}{month}{year}"] is string cacheContents) return cacheContents;
            
            // If we can't scrape numbers before 1 minute on a single page, begin to think of a different career. :)
            var policy = new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) };

            // Fetch the page contents.
            var browser = new ScrapingBrowser() { AllowAutoRedirect = true };

            cacheContents = browser.NavigateToPage(new Uri(string.Format(entity.LottoUrl, entity.State, month, year))).Html
                .ChildNodes["html"].ChildNodes["body"].InnerText;

            // Sanity Check
            if (string.IsNullOrEmpty(cacheContents) ||
                cacheContents.Contains($"No {Utility.Sanitize(entity.State)} drawings on {month} {year}"))
                throw new FileNotFoundException("Page does not exist.");

            cache.Set($"{entity.StateID}{month}{year}", cacheContents, policy);

            return cacheContents;

        }
        private void ParseLottoHtml(LottoEntity entity, string innerHtml)
        {
            string strLine;
            var strReader = new StringReader(innerHtml);

            while ((strLine = strReader.ReadLine()) != null)
            {
                ParseLottoStringLine(entity, strLine);
            }
        }
        private void ParseLottoStringLine(LottoEntity entity, string strLine)
        {
            // Sanity Check
            // Is first character Alpha?
            if (string.IsNullOrWhiteSpace(strLine) ||
                !Regex.IsMatch(strLine[0].ToString(), "[A-Za-z]"))
                return;

            try
            {
                foreach (LottoGame lg in entity.LottoGames)
                {
                    // Name of the game will begin on index[0] for each string line.
                    if (strLine.IndexOf(lg.Game, StringComparison.Ordinal) != 0) continue;

                    // Date is always a length of 10
                    if (strLine.Length <= (lg.Game.Length + 10) ||
                        !DateTime.TryParse(strLine.Substring((lg.Game.Length), 10), out var dtLottDate)) continue;

                    var nPos = 0;
                    // Get array of numbers, trim it then insert it to database.                   
                    var numList = strLine.Substring(lg.Game.Length + 10).Split(',').Select(s => s.Trim()).ToList();
                    
                    foreach (var strNum in numList)
                    {
                        if (int.TryParse(strNum, out var number))
                            LottoNumbers.Add(new LottoNumber(entity.StateID, lg.LottoGameID, number, dtLottDate, nPos++));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void EventLogEntry(string logEntry)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@AppID", CommonCore.LottoAppType.LottoNumbersService),
                new SqlParameter("@LogEntry", logEntry),               
            };

            try
            {
                Database.ExecuteNonQuery(CommonCore.spInsertLottoAppLog, parameters.ToArray());
                Utility.LogMessageToFile(logEntry);
            }
            catch (Exception e)
            {
                // eat it...
                Utility.LogMessageToFile(e.Message + ": " + e.InnerException?.Message);
            }            
        }
    }         
}

