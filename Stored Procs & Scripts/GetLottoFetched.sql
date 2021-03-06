
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	Return LottoState's LottoGame
-- Test: EXEC [dbo].[_spGetLottoFetched]
-- =============================================

IF object_id('[dbo].[_spGetLottoFetched]') IS NULL
    EXEC ('create procedure [dbo].[_spGetLottoFetched] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLottoFetched]
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT lf.[StateID], lf.[GameID], lf.[LottoDate]		  
	FROM [dbo].[LottoFetched] AS lf
	
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spGetLottoFetched]
    TO Guest;  
GO
