
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	Return LottoState's LottoGame Enabled
-- Test: EXEC [dbo].[_spGetLottoStateGameEnabled] 4, 12
-- =============================================

IF object_id('[dbo].[_spGetLottoStateGameEnabled]') IS NULL
    EXEC ('create procedure [dbo].[_spGetLottoStateGameEnabled] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLottoStateGameEnabled]
@StateID [INT],
@GameID [INT]
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
	IF EXISTS (SELECT 1
	FROM [dbo].[LottoStateGame] AS lg
	WHERE lg.StateID = @StateID AND
	      lg.LottoGameID = @GameID)
		  SELECT 1 AS [Exists]	
	ELSE
		  SELECT 0 AS [Exists]		  
	END
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spGetLottoStateGameEnabled]
    TO Guest;  
GO
