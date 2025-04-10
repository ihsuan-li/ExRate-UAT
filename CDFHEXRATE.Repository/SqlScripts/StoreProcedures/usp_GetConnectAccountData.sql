USE [CDFH_EXRATE]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetConnectAccountData]    Script Date: 2025/3/5 上午 09:46:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetConnectAccountData]
	-- Add the parameters for the stored procedure here
    @SqlSysNo NVARCHAR(50),
    @SqlAccount NVARCHAR(50),
    @DBName NVARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	  FROM ConnectAccountData
	  RETURN;
END
GO

