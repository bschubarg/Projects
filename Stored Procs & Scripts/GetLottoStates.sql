
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2017
-- Description:	Return Enabled LottoStates
-- Test: EXEC [dbo].[_spGetLottoStates]
-- =============================================

IF object_id('[dbo].[_spGetLottoStates]') IS NULL
    EXEC ('create procedure [dbo].[_spGetLottoStates] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLottoStates]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT lsn.[StateID], lsn.[Name] AS [State], lg.GameID AS GameID, lg.Game AS Game, lu.LottoTemplate AS LottoUrl
	FROM [dbo].[LottoUrlStateName] AS lsn
	INNER JOIN [dbo].[LottoStateGame] AS lsg ON lsg.StateID = lsn.StateID
	INNER JOIN [dbo].[LottoGame] AS lg ON lg.GameID = lsg.LottoGameID
	INNER JOIN [dbo].[LottoUrls] AS lu ON lu.ID = lsg.LottoUrlID
	WHERE lsn.[Enable] = 1 AND lsg.[Enable] = 1
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spGetLottoStates]
    TO Guest;  
GO
