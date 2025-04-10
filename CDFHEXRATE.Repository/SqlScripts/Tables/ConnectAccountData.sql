/*
 Source Server         : 開發金控 開發資本 凱基 (10.20.30.201)(DEV)
 Source Server Type    : SQL Server
 Source Server Version : 15004312 (15.00.4312)
 Source Host           : 10.20.30.201:1433
 Source Catalog        : CDFH_EXRATE
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 15004312 (15.00.4312)
 File Encoding         : 65001

 Date: 05/03/2025 09:43:41
 Purpose: 使用於模擬金控 UPDB 資料
*/


-- ----------------------------
-- Table structure for ConnectAccountData
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectAccountData]') AND type IN ('U'))
	DROP TABLE [dbo].[ConnectAccountData]
GO

CREATE TABLE [dbo].[ConnectAccountData] (
  [ServerName] nchar(50) COLLATE Chinese_Taiwan_Stroke_CS_AS  NULL,
  [DatabaseName] nchar(50) COLLATE Chinese_Taiwan_Stroke_CS_AS  NULL,
  [ConnectAccount] nchar(50) COLLATE Chinese_Taiwan_Stroke_CS_AS  NULL,
  [Password] nchar(50) COLLATE Chinese_Taiwan_Stroke_CS_AS  NULL
)
GO

ALTER TABLE [dbo].[ConnectAccountData] SET (LOCK_ESCALATION = TABLE)
GO

