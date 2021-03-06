
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	INSERT Lotto Log Entry for a given App ID
-- Test: EXEC [dbo].[_spInsertLottoAppLog] 0, 'Test Log'
-- =============================================

IF object_id('[dbo].[_spInsertLottoAppLog]') IS NULL
    EXEC ('create procedure [dbo].[_spInsertLottoAppLog] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo]._spInsertLottoAppLog
	@AppID [int],
	@LogEntry nvarchar (200) = null
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
   BEGIN
    INSERT INTO LottoLog( LottoAppID, LogEntry)
	VALUES (@AppID, @LogEntry);
   END	   
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spInsertLottoAppLog]
    TO Guest;  
GO
