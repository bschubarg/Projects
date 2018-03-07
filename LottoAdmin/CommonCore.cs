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
// Copyright (c) 2016 William Schubarg

using System;
using System.Collections.Generic;

namespace LottoCommon
{
    public class CommonCore
    {
        // Stored Procedures used in the windows service & WebAPI
        public static readonly string spGetLottos = "_spGetLottoStates";
        public static readonly string spGetLottoNumbers = "_spGetLottoNumbers";
        public static readonly string spGetLottoNumbersByType = "_spGetLottoNumbersByType";
        public static readonly string spInsertLottoNumber = "_spInsertLottoNumber";
        public static readonly string spTableDataExistByType = "_spTableDataExistByType";
        public static readonly string spGetTriggerTime = "_spGetLottoAppSettings";
        public static readonly string spGetLottoFetched = "_spGetLottoFetched";
        public static readonly string spInsertLottoFetched = "_spInsertLottoFetched";
        public static readonly string spIsLottoGameFetched = "_spIsLottoGameFetched";
        public static readonly string spInsertLottoAppLog = "_spInsertLottoAppLog";
        public static readonly string spGetLatestLottoLogs = "_spGetLatestLottoLogs";
        // Tables
        public static readonly string tblLottoNumber = "LottoNumber";
        public static readonly string tblLottoFetched = "LottoFetched"; 
        // SQL Commands
        // Data Members
        public const string LottoNumberServiceLogFile = "Lotto_Service_Log.txt";
        public const string MyDelimiter = "*-*";

        // Reverse ordered Months for interation
        public static readonly string[] MonthStrings = { "Dec", "Nov", "Oct", "Sep", "Aug", "Jul", "Jun", "May", "Apr", "Mar", "Feb", "Jan" };
        //Enums
        [Serializable]
        public enum LottoAppType
        {
            LottoNumbersService = 0,
            LottoAdmin
        }

        [Serializable]
        public enum LottoErrorMethod
        {
            InitializeComponents = 500,
            OnStart = 1000,
            HandleTimer = 2000,
            RetrieveLottoNumbers = 3000,
            LoadLottoEntities = 4000,
            InsertLottoNumbers = 5000
        }

        [Serializable]
        public enum LottoParameterType
        { // Look Familiar?
            BigInt = 0,
            Binary = 1,
            Bit = 2,
            Char = 3,
            DateTime = 4,
            Decimal = 5,
            Float = 6,
            Int = 8,
            Money = 9,
            SmallDateTime = 15,
            SmallInt = 16,
            SmallMoney = 17,
            Text = 18,
            VarChar = 22,
            Date = 31
        }
    }

    [Serializable]
    public class LottoAppSetting
    {
        public DateTime TriggerFetch { get; set; }
    }

    [Serializable]
    public class LottoFetch
    {
        public int StateID { get; set; }
        public int GameID { get; set; }
        public DateTime LottoDate { get; set; }
    }
    
    [Serializable]
    public class Lotto
    {
        public int StateID { get; set; }
        public string State { get; set; }
        public int GameID { get; set; }
        public string Game { get; set; }
        public string LottoUrl { get; set; }
    }

    [Serializable]
    public class LottoParameter
    {
        // Constructors
        public LottoParameter() { }
        public LottoParameter(CommonCore.LottoParameterType lType){ LottoParamType = lType; }
        // Properties
        public CommonCore.LottoParameterType LottoParamType { get; set; }
        public string LottoParm { get; set; }
        public string LottoParmValue { get; set; }
    }

    [Serializable]
    public class LottoGame
    {
        // Constructors
        public LottoGame() { }
        public LottoGame(int gameId, string game) { LottoGameID = gameId; Game = game; }
        // Properties
        public int LottoGameID { get; set; }
        public int GameID { get; set; } // Primary Key
        public string Game { get; set; }        
    }

    [Serializable]
    public class LottoStateGame
    {
        // Constructor 
        public LottoStateGame() { }
        // Properties
        public int ID { get; set; }
        public int StateID { get; set; }             
        public int LottoGameID { get; set; }
        public int LottoUrlID { get; set; }
        public bool Enable { get; set; }
    }

    [Serializable]
    public class LottoUrlStateName
    {
        // Constructor 
        public LottoUrlStateName () { }
        // Properties
        public int StateID { get; set; }
        public string StateProvinceCode { get; set; }
        public string Name { get; set; }        
        public bool Enable { get; set; }
    }

    [Serializable]
    public class LottoEntity
    {
        // Constructors
        public LottoEntity() { LottoGames = new List<LottoGame>(); }
        public LottoEntity(int stateId, string state, string url) : this()
        {
            StateID = stateId;
            State = state;
            LottoUrl = url;
        }
        // Common data entities        
        public int StateID { get; set; }
        public string State { get; set; }
        public string LottoUrl { get; set; }
        public List<LottoGame> LottoGames { get; set; }
        
        // Methods
        public LottoEntity AddGame(int gameId,string game)
        {
            LottoGames.Add(new LottoGame(gameId, game));
            return this;
        }        
    }

    [Serializable]
    public class LottoEntities : List<LottoEntity>
    {
        // Constructors
        public LottoEntities(){}
        public LottoEntities(LottoEntity lottoEntity){}

        // Overload the Add method
        public new bool Add(LottoEntity lottoEntity)
        {
            try
            {
                // Sanity Check... 
                base.Add(lottoEntity);
            }
            catch // Swallow everything...
            {
                return false;
            }

            return true;
        }
        // Methods
        public bool AddLotto(Lotto lotto) {

            var result = this.Find(x => x.StateID == lotto.StateID);

            if (result == null)
                Add(new LottoEntity(lotto.StateID, lotto.State, lotto.LottoUrl).AddGame(lotto.GameID, lotto.Game));
            else
                result.AddGame(lotto.GameID, lotto.Game);

            return true;
        }
    }

    [Serializable]
    public class LottoNumber
    {
        // Properties
        public int StateID { get; set; }
        public int GameID { get; set; }
        public int Number { get; set; }
        public DateTime? LottoDate { get; set; }
        public int HashCode { get; set; }

        // Constructor
        public LottoNumber()
        {
            Number = -1;
            LottoDate = null;
        }
        // Overloads
        public LottoNumber(int stateId, int gameId, int number, DateTime lottoDate, int pos) : this()
        {
            StateID = stateId;
            GameID = gameId;
            Number = number;
            LottoDate = lottoDate;
            HashCode = Hash.Base
                            .HashObject(StateID)
                            .HashObject(GameID)
                            .HashObject(LottoDate)
                            .HashObject(Number)
                            .HashObject(pos);
        }
        
        public override int GetHashCode()
        {
            return Hash.Base
                .HashObject(StateID)
                .HashObject(GameID)
                .HashObject(LottoDate)
                .HashValue(Number);
        }
    }

    public class LottoNumbers : List<LottoNumber>
    {
        public LottoNumbers() { }

        public LottoNumbers(LottoNumber lottoNumber)
        {
            //Add(lottoNumber);
        }

        // Overload the Add method
        public new bool Add(LottoNumber lottoNumber)
        {
            try
            {
                // Sanity Check... No number should exceed value of 99
                if (lottoNumber.Number > 99) return false;

                base.Add(lottoNumber);
            }
            catch // Swallow everything...
            {
                return false;
            }

            return true;
        }        
    }

    public class LottoNumberComparer : IEqualityComparer<LottoNumber>
    {
        // Overload Equals
        public bool Equals(LottoNumber x, LottoNumber y)
        {
            //Check whether the objects are the same object. 
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) ||
                ReferenceEquals(y, null))
                return false;

            // Check whether the LottoNumbers' properties are equal. 
            return x.LottoDate.ToString().Equals(y.LottoDate.ToString()) &                   
                   x.Number.Equals(y.Number) &
                   x.StateID.Equals(y.StateID) &
                   x.GameID.Equals(y.GameID);
        }

        public int GetHashCode(LottoNumber obj)
        {
            //Get hash code for the Name field if it is not null. 
            return obj.LottoDate == null ? 0 : obj.GetHashCode();
        }
    }

    // Fluent implementation of the algorithm posted above by Jon Skeet, 
    // but which includes no allocations or boxing operations.
    public static class Hash
    {
        public const int Base = 17;

        public static int HashObject(this int hash, object obj)
        {
            unchecked { return hash * 23 + (obj?.GetHashCode() ?? 0); }
        }

        public static int HashValue<T>(this int hash, T value)
            where T : struct
        {
            unchecked { return hash * 23 + value.GetHashCode(); }
        }
    }    
}
