-- =============================================
-- 建立 SignInRule 表
-- 用於設定簽到規則和獎勵
-- ⚠️ 注意：此腳本僅供參考。實際表格由 EF Migration 管理，請使用 dotnet ef database update
-- =============================================

USE [GameSpacedatabase]
GO

SET QUOTED_IDENTIFIER ON
GO

-- 檢查表是否已存在，如果存在則刪除
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SignInRule]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[SignInRule]
END
GO

-- 建立 SignInRule 表
CREATE TABLE [dbo].[SignInRule] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [SignInDay] INT NOT NULL,
    [Points] INT NOT NULL DEFAULT 0,
    [Experience] INT NOT NULL DEFAULT 0,
    [HasCoupon] BIT NOT NULL DEFAULT 0,
    [CouponTypeCode] NVARCHAR(50) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2(7) NULL,
    [Description] NVARCHAR(500) NULL,
    CONSTRAINT [PK_SignInRule] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_SignInRule_SignInDay]
ON [dbo].[SignInRule] ([SignInDay] ASC)
WHERE [IsActive] = 1
GO

CREATE NONCLUSTERED INDEX [IX_SignInRule_IsActive]
ON [dbo].[SignInRule] ([IsActive] ASC)
GO

-- 插入預設簽到規則資料
INSERT INTO [dbo].[SignInRule] ([SignInDay], [Points], [Experience], [HasCoupon], [CouponTypeCode], [IsActive], [Description])
VALUES
    (1, 10, 5, 0, NULL, 1, N'第一天簽到獎勵'),
    (2, 10, 5, 0, NULL, 1, N'第二天簽到獎勵'),
    (3, 15, 8, 0, NULL, 1, N'第三天簽到獎勵'),
    (4, 15, 8, 0, NULL, 1, N'第四天簽到獎勵'),
    (5, 20, 10, 0, NULL, 1, N'第五天簽到獎勵'),
    (6, 20, 10, 0, NULL, 1, N'第六天簽到獎勵'),
    (7, 30, 15, 1, 'WEEK_BONUS', 1, N'第七天簽到獎勵 + 週獎勵優惠券'),
    (14, 50, 25, 1, 'TWO_WEEK_BONUS', 1, N'連續簽到 14 天獎勵'),
    (21, 80, 40, 1, 'THREE_WEEK_BONUS', 1, N'連續簽到 21 天獎勵'),
    (30, 150, 75, 1, 'MONTH_BONUS', 1, N'連續簽到 30 天獎勵（滿月獎勵）')
GO

PRINT '✓ SignInRule 表建立成功'
PRINT '✓ 已插入 10 筆預設簽到規則'
GO
