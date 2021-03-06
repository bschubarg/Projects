
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	INSERT LottoState's LottoGame
-- Test: EXEC [dbo].[_spInsertLottoStateGame]
-- =============================================

IF object_id('[dbo].[_spInsertLottoStateGame]') IS NULL
    EXEC ('create procedure [dbo].[_spInsertLottoStateGame] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spInsertLottoStateGame]
	@StateID [int],
	@GameID [int],
	@LottoUrlID [int],
	@Enable[tinyint]
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT StateID FROM LottoStateGame
                   WHERE StateID = @StateID
                   AND LottoGameID = @GameID
				   AND LottoUrlID = @LottoUrlID
                   )
   BEGIN
    INSERT INTO LottoStateGame( StateID, LottoGameID, LottoUrlID, [Enable], ModifiedDate )
	VALUES (@StateID, @GameID, @LottoUrlID, @Enable, GETDATE());
   END	
   ELSE
   BEGIN
   UPDATE LottoStateGame SET [Enable] = @Enable, ModifiedDate = GETDATE()
   WHERE StateID = @StateID
                   AND LottoGameID = @GameID
				   AND LottoUrlID = @LottoUrlID
   END
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spInsertLottoStateGame]
    TO Guest;  
GO
