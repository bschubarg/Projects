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
using System.IO;
using System.Linq;
using LottoCommon;
using ScrapySharp.Network;

namespace LottoNumbersService
{
    public static class RetrieveLottoNumbers
    {
        static int nCount;
        static readonly List<LottoEntity> LottoList;

        static RetrieveLottoNumbers()
        {
            try
            {                
                LottoList = new List<LottoEntity>();
                Database.LoadEntities<LottoEntity>(CommonCore.spGetLottos, out LottoList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        internal static int LoadLottoEntities()
        {
            try
            {                                                
                return ScrapeLottoPages(LottoList);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) 
                    throw new Exception(ex.Message + ": " + ex.InnerException.Message);
                
                throw new Exception(ex.Message);
            }
        }

        private static int ScrapeLottoPages(List<LottoEntity> lstEntities)
        {
            nCount = 0;
            // Scrape the lotto url[s] and get count of added rows to database.
	        foreach (LottoEntity entity in lstEntities)
	        {
                ScrapeLottoPage(entity);
	        }

            return nCount;
        }

        private static void ScrapeLottoPage(LottoEntity entity)
        {   // Third party shareware DLL to get web page text.  Why reinvent the wheel?   
            // Scrapy Sharp is an open source scrape framework that combines a web client
            // able to simulate a web browser, and an HtmlAgilityPack extension to select 
            // elements using css selector (like JQuery). Scrapysharp greatly reduces the 
            // workload, upfront pain and setup normally involved in scraping a web-page.
            // https://www.codeproject.com/Articles/1041115/Webscraping-with-Csharp

            ScrapingBrowser Browser = new ScrapingBrowser();

            Browser.AllowAutoRedirect = true; // Browser has settings you can access in setup
            Browser.AllowMetaRedirect = true;

            try
            {
                // It is possible that during the new year, the web page is not constructed.
                // Make another attempt and when the new page is created, the is will pass.
                // Kinda expensive since if the page does not exist, could add a second or to
                // in the process.  Better idea?
                var bNewPage = true;
                var lottoYear = DateTime.Now.Year;

                while (true)
                {
                    try
                    {
                        WebPage pageResult = Browser.NavigateToPage(new Uri(string.Format(entity.LottoUrl, lottoYear--)));
                        var innerHtml = pageResult.Html.ChildNodes["html"].ChildNodes["body"].InnerText;

                        nCount += ParseLottoHtml(entity, innerHtml);
                    }
                    catch (Exception)
                    {
                        if (!bNewPage)
                            break; // Could not connect to the web page or internet (maybe...).

                        bNewPage = false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) 
                    throw new Exception(ex.Message + ": " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }           
        }

        private static int ParseLottoHtml(LottoEntity entity, string innerHtml)
        {            
            // If the separator parameter is null or contains no characters,
            // white-space characters are assumed to be the delimiters. 
            // White-space characters are defined by the Unicode standard 
            // and return true if they are passed to the Char.IsWhiteSpace method.
            int startIndex = 0;
            string[] inputArray = innerHtml.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);//split on a whitespace
            string[] templateArray = entity.LottoTemplate.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);//split on a whitespace
                        
            // First lets determine if we are barking up the right tree
            if (!(inputArray.Length != 0 &&
                templateArray.Length != 0 &&
                inputArray.Contains(templateArray[0]) &&
                (startIndex = GetStartIndex(inputArray, templateArray)) != -1))
                return 0;
            
            var arrayLength = inputArray.Length - startIndex;
            var lottoNumbers = new LottoNumbers(entity);
            
            // arrayIndex marks the beginning of the string to 'Scrape'
            // Every 4 elements is a date.  The next 6 are the lotto numbers
            // The 11th element to ... is the prize $ may or may not be 
            // followed by the character 'R' (Rollover).
            do
            {
                // We will take chunks of 10 elements.  
                // [0] - Day of week, [1] - Day, [2] - Month, [3] - Year, [4-9] - Lotto Numbers.
                // Saturday 24th December 2016 5 8 13 15 39 42 $8,750,000 R
                startIndex += GetDateAndLottoNumbers(inputArray.RangeSubset(startIndex, arrayLength), ref lottoNumbers);

            } while (startIndex < arrayLength);            

            return InsertLottoNumbers(lottoNumbers);
        }

        private static int GetStartIndex( string[] inputArray, IReadOnlyList<string> templateArray)
        {
            // Find the start delimiter.  The delimiter can be one or more values.            
            var i = 0;
            var y = 0;
            int[] beginArray = inputArray.FindAllIndex( x => x == templateArray[0]);
            
            for (; i < beginArray.Length; i++)            
            {
                if (templateArray[i] == inputArray[beginArray[i]])
                {                    
                    for (; y < templateArray.Count; y++)
                    {
                        if (templateArray[i] != inputArray[beginArray[i]])
                            break;
                    }
                    // The value of 'y' should be the length of the templateArray.Length
                    if (y == templateArray.Count)
                        return beginArray[i] + y;
                }                 
            }

            return -1;
        }

        /// <summary>
        /// Get the Day of week elements of the Subset and begin parsing
        /// subsequent elements for date and lotto numbers to end of array.
        /// </summary>
        /// <param name="rangeSubset"></param>
        /// <param name="lottoNumbers"></param>
        /// <returns>Last element read.</returns>
        private static int GetDateAndLottoNumbers(string[] rangeSubset, ref LottoNumbers lottoNumbers)
        {
            int index = 0;
            
            try
            {                
                int rangeLength = rangeSubset.Length;

                do
                {
                    switch (rangeSubset[index].ToLower())
                    {
                        case "sunday":
                        case "monday":
                        case "tuesday":
                        case "wednesday":
                        case "thursday":
                        case "friday":
                        case "saturday":
                        {
                            // [0] - Day of week, [1] - Day, [2] - Month, [3] - Year, [4-9] - Lotto Numbers.
                            // Saturday 24th December 2016 5 8 13 15 39 42 $8,750,000 R
                            
                            // We know that the following elements are the date
                            var lottoDate = DateTime.Parse(string.Format("{1} {0}, {2}", 
                                                                rangeSubset[++index].Remove(rangeSubset[index].Length - 2),
                                                                rangeSubset[++index], 
                                                                rangeSubset[++index]));
                            
                            // When we get here, the next should be the lotto numbers
                            int number;
                            int element = 0;
                            while (int.TryParse(rangeSubset[++index], out number))
                            {                                
                                lottoNumbers.Add(lottoDate, number, IsXtraNumber(lottoNumbers.lottoEntity, lottoDate, ++element) );                                
                            }                            
                        }
                            break;
                        default:
                            index++;
                            break;
                    }
                } while (index < rangeLength);             
            }
            catch (Exception) // Swallow any catches
            {                                
            }            

            return index;
        }
        /// <summary>
        /// Get the date when the lotto changed it format to using the Extra number
        /// </summary>
        /// <param name="lottoEntity"></param>
        /// <param name="lottoDate"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool IsXtraNumber(LottoEntity lottoEntity, DateTime lottoDate, int element)
        {
            // Test to see if there is a XtraBall
            if (lottoEntity.LottoXtraBall == 0 ||
                lottoEntity.LottoXtraBall != element)
                return false;

            return lottoDate >= lottoEntity.LottoXtraDate;
        }

        /// <summary>
        /// Compare the lists of what currently is in the database and what we retrieved from
        /// the web page.  Bulk insert the difference.
        /// </summary>
        /// <param name="lottoNumbers"></param>
        private static int InsertLottoNumbers(LottoNumbers lottoNumbers)
        {
            try
            {
                // Sanity check
                if (lottoNumbers == null ||
                    lottoNumbers.Count == 0)
                    return 0;

                var lottoNumberList = new List<LottoNumber>();
                
                var paramList = new List<Tuple<int, string, int>>
                {
                    new Tuple<int, string, int>((int) CommonCore.LottoParameterType.Int, "LottoType",
                        (int) lottoNumbers.lottoEntity.LottoType)
                };

                Database.LoadEntities(CommonCore.spGetLottoNumbersByType, out lottoNumberList, paramList);
                
                var tblDifference = lottoNumbers.Except(lottoNumberList, new LottoNumberComparer()).ConvertToDataTable();

                // If this is the first time running the service, populate with scraped numbers
                return Database.InsertEntities<LottoNumber>(CommonCore.tblLottoNumber,
                    (lottoNumberList.Count == 0) ? lottoNumbers.ConvertToDataTable() : tblDifference);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) 
                    throw new Exception(ex.Message + ": " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }       
    }
}
