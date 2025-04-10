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

 Date: 05/03/2025 09:43:32
*/


-- ----------------------------
-- Table structure for ExRate
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ExRate]') AND type IN ('U'))
	DROP TABLE [dbo].[ExRate]
GO

CREATE TABLE [dbo].[ExRate] (
  [DataDate] date  NOT NULL,
  [ExRateType] varchar(2) COLLATE Chinese_Taiwan_Stroke_CS_AS  NOT NULL,
  [ReferenceCurrency] varchar(10) COLLATE Chinese_Taiwan_Stroke_CS_AS  NOT NULL,
  [LocalBidExRate] decimal(20,8) DEFAULT 0 NOT NULL,
  [LocalOfferExRate] decimal(20,8) DEFAULT 0 NOT NULL,
  [LocalSettleExRate] decimal(20,8) DEFAULT 0 NOT NULL,
  [USDBidExRate] decimal(20,8) DEFAULT 0 NOT NULL,
  [USDOfferExRate] decimal(20,8) DEFAULT 0 NOT NULL,
  [USDSettleExRate] decimal(20,8) DEFAULT 0 NOT NULL,
  [SyncTime] datetime  NOT NULL
)
GO

ALTER TABLE [dbo].[ExRate] SET (LOCK_ESCALATION = TABLE)
GO


EXEC sp_addextendedproperty
'MS_Description', N'資料日期',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'DataDate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'類型: 1早盤、2收盤',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'ExRateType'
GO

EXEC sp_addextendedproperty
'MS_Description', N'交易國別',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'ReferenceCurrency'
GO

EXEC sp_addextendedproperty
'MS_Description', N'台幣報價買入',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'LocalBidExRate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'台幣報價賣出',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'LocalOfferExRate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'台幣報價作帳',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'LocalSettleExRate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'美元報價買入',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'USDBidExRate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'美元報價賣出',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'USDOfferExRate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'美元報價作帳',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'USDSettleExRate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'排程寫入時間',
'SCHEMA', N'dbo',
'TABLE', N'ExRate',
'COLUMN', N'SyncTime'
GO


-- ----------------------------
-- Primary Key structure for table ExRate
-- ----------------------------
ALTER TABLE [dbo].[ExRate] ADD CONSTRAINT [PK_ExRate] PRIMARY KEY CLUSTERED ( [DataDate], [ExRateType], [ReferenceCurrency])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

