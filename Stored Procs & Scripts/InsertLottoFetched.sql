
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	Return LottoState's LottoGame
-- Test: EXEC [dbo].[_spInsertLottoFetched] 4, 2, '01-01-2002'
-- =============================================

IF object_id('[dbo].[_spInsertLottoFetched]') IS NULL
    EXEC ('create procedure [dbo].[_spInsertLottoFetched] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spInsertLottoFetched]
@StateID [INT],
@GameID [INT],
@LottoDate [DateTime]
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN
		INSERT INTO LottoFetched( StateID, GameID, LottoDate)
		VALUES (@StateID, @GameID, @LottoDate);
   END			
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spInsertLottoFetched]
    TO Guest;  
GO
