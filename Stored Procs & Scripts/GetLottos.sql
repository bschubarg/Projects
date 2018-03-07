-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2016
-- Description:	Return LottoTypes, LottoNames, 
--    LottoTemplate, LottoXtraDate, LottoXtraBall, State & LottoURL
-- Test: EXEC [dbo].[_spGetLottos]
-- =============================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id('[dbo].[_spGetLottos]') IS NULL
    EXEC ('create procedure dbo.[dbo].[_spGetLottos] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLottos]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT ls.[State], 
		   ls.LottoType, 
		   ls.LottoName, 
		   ls.LottoUrl, 
		   ls.LottoXtraDate, 
		   ls.LottoXtraBall, 
		   lp.LottoTemplate, 
		   ls.LottoBalls 
		   
	FROM [dbo].[LottoType] AS lt
	
	INNER JOIN LottoState AS ls	
		ON lt.LottoType = ls.LottoType
	LEFT OUTER JOIN LottoTemplate lp
		ON lt.LottoType = lp.LottoType

	WHERE lt.[Enable] = 1 -- True

END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spGetLottos]
    TO Guest;  
GO
