USE [CDFH_EXRATE]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetAllExRateData]    Script Date: 2025/3/5 ¤W¤È 09:45:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAllExRateData]
	-- Add the parameters for the stored procedure here
   -- @CountryShortName NVARCHAR(50),
    @date NVARCHAR(50),
    @type NVARCHAR(50),
    @striReferenceCurrency NVARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	  FROM ExRate_Src
	 WHERE  DataDate = @date
	   AND ExRateType = @type
	  RETURN;
END
GO


