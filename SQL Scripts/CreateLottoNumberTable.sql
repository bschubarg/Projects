USE [StateLottoNumbers]
GO

ALTER TABLE [dbo].[LottoNumber] DROP CONSTRAINT [DF_LottoNumber1_EntryDate]
GO

/****** Object:  Table [dbo].[LottoNumber]    Script Date: 1/15/2018 3:45:00 PM ******/
DROP TABLE [dbo].[LottoNumber]
GO

/****** Object:  Table [dbo].[LottoNumber]    Script Date: 1/15/2018 3:45:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LottoNumber](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StateID] [int] NOT NULL,
	[GameID] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[LottoDate] [datetime] NOT NULL,
	[EntryDate] [datetime] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LottoNumber] ADD  CONSTRAINT [DF_LottoNumber1_EntryDate]  DEFAULT (getdate()) FOR [EntryDate]
GO

CREATE UNIQUE INDEX MyIndex ON [dbo].[LottoNumber](StateID, GameID, Number, LottoDate) WITH IGNORE_DUP_KEY
GO



