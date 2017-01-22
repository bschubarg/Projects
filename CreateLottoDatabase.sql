-- =============================================
-- Author: William Schubarg
-- Create date: 1/21/2017
-- Description:	Create the SQL Database and tables to support
--    the Lotto Web Application and Service.
-- Test:  
-- =============================================

-- CREATE Database
DECLARE @sql NVARCHAR(MAX);
-- Database name: Make sure it matches what the connection string has in the config files.
DECLARE @dbname NVARCHAR(50) = N'StateLottoNumbers2'
DECLARE @dropdbname NVARCHAR(50) = N'DROP DATABASE ' + @dbname

-- Kill all connections then Drop Database
WHILE EXISTS(select NULL from sys.databases where name = @dbname )
BEGIN	
    DECLARE @KillConnections NVARCHAR(MAX)
    SELECT @KillConnections = COALESCE(@KillConnections,'') + 'Kill ' + Convert(varchar, SPId) + ';' 
	FROM MASTER..SysProcesses WHERE DBId = DB_ID(@dbname) AND SPId <> @@SPId	
    EXEC(@KillConnections)
    EXEC(@dropdbname)
END

SET @sql = N'CREATE DATABASE ' + @dbname

EXEC sp_executesql @sql;
 
-- Create Tables and populate the necessary data columns

SET @sql = N'CREATE TABLE ' + @dbname + '.[dbo].[LottoNumber](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LottoType] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[XtraNumber] [bit] NOT NULL CONSTRAINT [DF_LottoNumber_XtraNumber]  DEFAULT ((0)),
	[LottoDate] [date] NOT NULL,
	[EntryDate] [date] NOT NULL CONSTRAINT [DF_LottoNumbers_EntryDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_LottoNumbers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'

EXEC sp_executesql @sql;

SET @sql = N'CREATE TABLE ' + @dbname + '.[dbo].[LottoState](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[State] [varchar](2) NOT NULL,
	[LottoType] [int] NOT NULL,
	[LottoName] [varchar](50) NULL,
	[LottoUrl] [varchar](500) NOT NULL,
	[LottoXtraDate] [date] NULL,
	[LottoXtraBall] [int] NOT NULL CONSTRAINT [DF_LottoState_LottoXtraBall]  DEFAULT ((0)),
	[LottoDescription] [varchar](1000) NULL,
 CONSTRAINT [PK_LottoState] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'

EXEC sp_executesql @sql;

SET @sql = N'CREATE TABLE ' + @dbname +'.[dbo].[LottoTemplate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LottoType] [int] NOT NULL,
	[LottoTemplate] [varchar](500) NOT NULL,
 CONSTRAINT [PK_LottoTemplate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'
EXEC sp_executesql @sql;

SET @sql = N'CREATE TABLE ' + @dbname + '.[dbo].[LottoType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LottoType] [int] NOT NULL,
	[Enable] [bit] NOT NULL CONSTRAINT [DF_LottoType_Enable]  DEFAULT ((1)),
 CONSTRAINT [PK_LottoType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'

EXEC sp_executesql @sql;

/*
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'States that are supported: Florida, Califronia ... etc'' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'State'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pick 4, Fantasy 6, Powerball ... etc' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'URL of the web page to scrape' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date when the extra number was added or that the last number is the extra number.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoXtraDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The extra number in relation to the LottoNumbers.  Usually the last number(ball). ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoXtraBall'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Determine the string format to retrieve values from rendered web page in memory.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoTemplate', @level2type=N'COLUMN',@level2name=N'LottoTemplate'
GO
*/

-- INSERT SOME DATA INTO TABLES
SET @sql = 'INSERT INTO ' + @dbname + '.[dbo].[LottoType]
           ([LottoType]
           ,[Enable])
     VALUES
           (1,1), (2,0), (3,0),(4,1),(5,1),(6,1),(7,1),(8,1)'
EXEC(@sql)

SET @sql = 'INSERT INTO ' + @dbname + '.[dbo].[LottoTemplate]
           ([LottoType]
           ,[LottoTemplate])
     VALUES
           (1, ''Result Date Result Jackpot''),
           (4, ''Result Date Result Jackpot''),
		   (5, ''Result Date Result Jackpot''),
		   (6, ''Result Date Result Jackpot''),
		   (7, ''Result Date Result Jackpot''),
		   (8, ''Result Date Result Jackpot'')'
EXEC(@sql)

SET @sql = 'INSERT INTO ' + @dbname + '.[dbo].[LottoState]
([State]
,[LottoType]
,[LottoName]
,[LottoUrl]
,[LottoXtraDate]
,[LottoXtraBall]
,[LottoDescription])
VALUES
(''FL''
,1
,''Florida Lotto''
,''http://www.lottonumbers.com/florida-lotto-results-{0}.asp''
,''2009-10-14''
,7
,''Florida Lotto; Pick 6 numbers 1-53. The Florida Lotto is drawn twice a week on Wednesday and Saturday _
and has a minimum jackpot of $2 million. Players are required to match six numbers from a _
possible 53 in order to win the jackpot and there are an additional three prize tiers for players who match _
fewer numbers.  Florida Lotto players also have the option to add XTRA to their game for a small additional _
stake. The XTRA number, from 2 to 5, is drawn at random following the main draw and this number determines _
how much the non-jackpot cash prizes will increase.''),
(''FL''
,2
,''Florida Fantasy 5''
,''http://''
,NULL
,0
,''Florida Fantasy 5; Pick 5 numbers 1-36''),
(''FL''
,3
,''Florida Cash 3''
,''http://''
,NULL
,0
,''Florida Cash 3; Pick 3 numbers 0-9''),
(''TX''
,4
,''Lotto Texas ''
,''http://www.lottonumbers.com/lotto-texas-results-{0}.asp''
,NULL
,0
,''Lotto Texas plays twice a week on Wednesday and Saturday and requires players to match six _
numbers from a possible 54 in order to win the jackpot, which starts at $4 million.Lotto Texas _
players also have the option to add XTRA to their game for a small additional _
stake. This pays increased prizes to non-jackpot prize winners and also offers a prize to players _
matching just two numbers.''),
(''IL''
,5
,''Illinios Lotto''
,''http://www.lottonumbers.com/illinois-lotto-results-{0}.asp''
,''2012-10-31''
,7
,''The Illinois Lottery is drawn on Monday, Thursday and Saturday and requires players to match six _
numbers from a possible 52 in order to win the jackpot. Players can also opt to play an additional _
number, named Extra Shot, to increase their non-jackpot winnings. The standard game has five prize _
tiers including the jackpot but Lotto Extra Shot offers eleven prize tiers. The Illinois Lottery has _
a starting jackpot of $2 million and no jackpot cap.''),
(''NY''
,6
,''New York Lotto''
,''http://www.lottonumbers.com/new-york-lotto-results-{0}.asp''
,''1978-11-11''
,7
,''The New York Lotto has a starting jackpot of $2 million. Players are required to match _
six numbers from a possible 59 in order to win the jackpot and the draw takes place on Wednesday _
and Saturday.  A total of seven numbers are drawn, with the final number acting as the Bonus _
number. There are a further three prize tiers for players matching three, four or five numbers, _
with all prize tiers being pari-mutuel, with the exception of the Match 3 prize.''),
(''NJ''
,7
,''New Jersey Pick Six''
,''http://www.lottonumbers.com/new-jersey-pick-six-lotto-results-{0}.asp''
,''2015-12-15''
,7
,''The New Jersey Pick Six Lotto is the main state lottery game played in New Jersey. It is drawn on _
a Monday and Thursday and requires players to match six numbers from a possible 49 in order to win _
the jackpot. Players can also win prizes for matching three, four or five numbers. There is a _
minimum jackpot of $2 million.''),
(''CA''
,8
,''California SuperLotto''
,''http://www.lottonumbers.com/california-superlotto-results-{0}.asp''
,''2000-06-07''
,6
,''The California SuperLotto Plus is the main state lottery played across California, which is drawn _
every Wednesday and Saturday. The California SuperLotto Plus is a two-drum game requiring players _
to match five numbers from a pool of 47, followed by one Mega number from a second pool of 27 in _
order to win the jackpot. The jackpot starts at $7 million.'')'

--PRINT @sql	     
EXEC(@sql)
