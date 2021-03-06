
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	Return Latest log entries.  For now just the last 20 entries
--              for a given LottoAppID
-- Test: EXEC [dbo].[_spGetLatestLottoLogs] 0
-- =============================================

IF object_id('[dbo].[_spGetLatestLottoLogs]') IS NULL
    EXEC ('create procedure [dbo].[_spGetLatestLottoLogs] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLatestLottoLogs]
@AppID [INT]
AS

BEGIN	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT TOP 2000 lg.[LogEntry],lg.[LogEntryDate]		  
	FROM [dbo].[LottoLog] AS lg
	WHERE lg.LottoAppID = @AppID
	ORDER BY lg.LogEntryDate DESC
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spGetLatestLottoLogs]
    TO Guest;  
GO
