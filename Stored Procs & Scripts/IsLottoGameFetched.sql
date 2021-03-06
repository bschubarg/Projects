
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	INSERT LottoState's LottoGame
-- Test: EXEC [dbo].[_spIsLottoGameFetched] 4, 12, '12-01-2018 00:00:00.000'
-- =============================================

IF object_id('[dbo].[_spIsLottoGameFetched]') IS NULL
    EXEC ('create procedure [dbo].[_spIsLottoGameFetched] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spIsLottoGameFetched]
	@StateID [int],
	@GameID [int],
	@LottoDate [datetime]	
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT StateID FROM LottoFetched
                   WHERE StateID = @StateID
                   AND GameID = @GameID
				   AND LottoDate = @LottoDate
                   )
   BEGIN
    SELECT 0 AS [Exists]
   END	
   ELSE
   BEGIN
	SELECT 1 AS [Exists]
   END
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spIsLottoGameFetched]
    TO Guest;  
GO
