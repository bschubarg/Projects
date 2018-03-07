
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2016
-- Description:	Return Lotto Application Settings.  This will
--   grow in the future.... for now... send sparse data.
-- Test: EXEC [dbo].[_spUpdateLottoAppSettings] 0, '2017-10-31 4:30'
-- =============================================

IF object_id('[dbo].[_spUpdateLottoAppSettings]') IS NULL
    EXEC ('create procedure [dbo].[_spUpdateLottoAppSettings] as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spUpdateLottoAppSettings]
	@LottoApp [int],
	@TriggerFetch [datetime]
AS

BEGIN
		
	IF NOT EXISTS (SELECT LottoAppID FROM LottoApp
                   WHERE LottoAppID = @LottoApp)
   BEGIN
    INSERT INTO LottoApp( LottoAppID, TriggerFetch)
	VALUES (@LottoApp, @TriggerFetch);
   END
   BEGIN
    UPDATE LottoApp SET TriggerFetch = @TriggerFetch, ModifiedDate = GETDATE()
    WHERE LottoAppID = @LottoApp                   
   END
END
GO

GRANT EXECUTE ON OBJECT::[dbo].[_spUpdateLottoAppSettings]
    TO Guest;  
GO
