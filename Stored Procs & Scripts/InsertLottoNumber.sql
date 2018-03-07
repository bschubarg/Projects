USE [StateLottoNumbers]
GO
/****** Object:  StoredProcedure [dbo].[_spInsertLottoNumber]    Script Date: 1/15/2018 8:02:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[_spInsertLottoNumber]
	@StateID [int],
	@GameID [int],
	@Number [int],
	@LottoDate [Date],
	@HashCode [int]
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interferring with SELECT statements.
	SET NOCOUNT ON;
	
	-- Sanity check
	IF @Number > 99 Return -1

	IF NOT EXISTS (SELECT StateID FROM LottoNumber
                   WHERE StateID = @StateID
                   AND GameID = @GameID
				   AND Number = @Number
                   AND LottoDate = @LottoDate
				   AND HashCode = @HashCode)
   BEGIN
    INSERT INTO LottoNumber( StateID, GameID, Number, LottoDate, HashCode )
	VALUES (@StateID, @GameID, @Number, @LottoDate, @HashCode);
   END	
END
