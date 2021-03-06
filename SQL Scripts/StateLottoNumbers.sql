USE [master]
GO
/****** Object:  Database [StateLottoNumbers]    Script Date: 3/12/2017 9:24:50 PM ******/
CREATE DATABASE [StateLottoNumbers]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LottoNumbers', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\LottoNumbers.mdf' , SIZE = 6144KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'LottoNumbers_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\LottoNumbers_log.ldf' , SIZE = 12352KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [StateLottoNumbers] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [StateLottoNumbers].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [StateLottoNumbers] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET ARITHABORT OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [StateLottoNumbers] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [StateLottoNumbers] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET  DISABLE_BROKER 
GO
ALTER DATABASE [StateLottoNumbers] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [StateLottoNumbers] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [StateLottoNumbers] SET  MULTI_USER 
GO
ALTER DATABASE [StateLottoNumbers] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [StateLottoNumbers] SET DB_CHAINING OFF 
GO
ALTER DATABASE [StateLottoNumbers] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [StateLottoNumbers] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [StateLottoNumbers]
GO
/****** Object:  Table [dbo].[LottoNumber]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LottoNumber](
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

GO
/****** Object:  Table [dbo].[LottoState]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LottoState](
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

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LottoTemplate]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LottoTemplate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LottoType] [int] NOT NULL,
	[LottoTemplate] [varchar](500) NOT NULL,
 CONSTRAINT [PK_LottoTemplate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LottoType]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LottoType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LottoType] [int] NOT NULL,
	[Enable] [bit] NOT NULL CONSTRAINT [DF_LottoType_Enable]  DEFAULT ((1)),
 CONSTRAINT [PK_LottoType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[_spGetLottoNumbers]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_spGetLottoNumbers]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT ln.LottoType, ln.[Number], ln.XtraNumber, ln.LottoDate FROM LottoNumber AS ln			
END

GO
/****** Object:  StoredProcedure [dbo].[_spGetLottoNumbersByType]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_spGetLottoNumbersByType]
	@LottoType [int]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT ln.LottoType, ln.[Number], ln.XtraNumber, ln.LottoDate, ln.EntryDate FROM LottoNumber AS ln
	
	INNER JOIN LottoType AS lt
		ON lt.LottoType = ln.LottoType
	
	WHERE @LottoType = ln.LottoType AND lt.[Enable] = 1 -- True
END

GO
/****** Object:  StoredProcedure [dbo].[_spGetLottos]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_spGetLottos]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT ls.LottoType, ls.[State], ls.LottoName, ls.LottoUrl, ls.LottoXtraDate, ls.LottoXtraBall, ls.LottoDescription, lp.LottoTemplate FROM LottoType AS lt
	
	INNER JOIN LottoState AS ls	
		ON lt.LottoType = ls.LottoType
	LEFT OUTER JOIN LottoTemplate lp
		ON lt.LottoType = lp.LottoType

	WHERE lt.[Enable] = 1 -- True
END

GO
/****** Object:  StoredProcedure [dbo].[_spGetLottoTypes]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_spGetLottoTypes]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT DISTINCT lt.LottoType, (SELECT COUNT(1) FROM LottoNumber WHERE lt.LottoType = ln.LottoType) AS [Count] FROM LottoType AS lt
	LEFT OUTER JOIN LottoNumber AS ln 
	ON ln.LottoType = lt.LottoType
	WHERE lt.[Enable] = 1
END

GO
/****** Object:  StoredProcedure [dbo].[_spInsertLottoNumber]    Script Date: 3/12/2017 9:24:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_spInsertLottoNumber]
	@LottoType [int],
	@Number [int],
	@LottoDate [Date]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT LottoType, Number, LottoDate FROM LottoNumber 
                   WHERE LottoType = @LottoType
                   AND Number = @Number
                   AND LottoDate = @LottoDate)
   BEGIN
    INSERT INTO LottoNumber( LottoType, Number, LottoDate )
	VALUES (@LottoType, @Number, @LottoDate);
   END	

	RETURN 0;
END

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'States that are supported: Florida, Califronia ... etc' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'State'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pick 4, Fanatsy 6, Powerball ... etc' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'URL of the web page to scrape' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date when the extra number was added or that the last number is the extra number.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoXtraDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The extra number in relation to the LottoNumbers.  Usually the last number(ball). ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoState', @level2type=N'COLUMN',@level2name=N'LottoXtraBall'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Determine the string format to retrieve values from rendered web page in memory.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LottoTemplate', @level2type=N'COLUMN',@level2name=N'LottoTemplate'
GO
USE [master]
GO
ALTER DATABASE [StateLottoNumbers] SET  READ_WRITE 
GO
