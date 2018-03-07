
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2016
-- Description:	Return Lotto Application Settings.  This will
--   grow in the future.... for now... send sparse data.
-- Test: EXEC [dbo].[_spGetLottoAppSettings] 0
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id('[dbo].[_spGetLottoAppSettings]') IS NULL
    EXEC ('create procedure dbo.[dbo].[_spGetLottoAppSettings] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLottoAppSettings]
	@LottoApp [int]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT la.TriggerFetch
	FROM [dbo].[LottoApp] AS la
	WHERE la.LottoAppID = @LottoApp		  

END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spGetLottoAppSettings]
    TO Guest;  
GO
