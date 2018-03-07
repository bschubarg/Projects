-- =============================================
-- Author: William Schubarg
-- Create date: 12/19/2016
-- Description:	Return LottoType, Number, XtraNumber, LottoDate
-- Test: EXEC _spGetLottoNumbers
-- =============================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF object_id('_spGetLottoNumbers') IS NULL
    EXEC ('create procedure dbo._spGetLottoNumbers as select 1')
	-- This way, security settings, comments and other meta deta will survive the deployment.
GO

ALTER PROCEDURE [dbo].[_spGetLottoNumbers]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT ln.LottoType, ln.[Number], ln.XtraNumber, ln.LottoDate FROM LottoNumber AS ln			
END
GO

GRANT EXECUTE ON OBJECT::[dbo]._spGetLottoNumbers
    TO Guest;  
GO
