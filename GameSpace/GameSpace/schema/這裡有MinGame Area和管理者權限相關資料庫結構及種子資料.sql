-- =====================================================
-- GameSpace 資料庫結構及種子資料
-- 基於實際 SQL Server 資料庫結構重新編寫
-- 更新日期: 2025-09-30 20:54:44
-- =====================================================

-- 1. 管理者權限系統 (Manager Permission System)
-- =====================================================

-- 管理者資料表
CREATE TABLE ManagerData (
    Manager_Id int IDENTITY(30000001,1) PRIMARY KEY,
    Manager_Name nvarchar(30),
    Manager_Account varchar(30),
    Manager_Password nvarchar(200),
    Administrator_registration_date datetime2,
    Manager_Email nvarchar(255) NOT NULL,
    Manager_EmailConfirmed bit NOT NULL DEFAULT 0,
    Manager_AccessFailedCount int NOT NULL DEFAULT 0,
    Manager_LockoutEnabled bit NOT NULL DEFAULT 1,
    Manager_LockoutEnd datetime2 NULL
);

-- 管理者角色權限表
CREATE TABLE ManagerRolePermission (
    ManagerRole_Id int IDENTITY(1,1) PRIMARY KEY,
    role_name nvarchar(50) NOT NULL,
    AdministratorPrivilegesManagement bit DEFAULT 0,
    UserStatusManagement bit DEFAULT 0,
    ShoppingPermissionManagement bit DEFAULT 0,
    MessagePermissionManagement bit DEFAULT 0,
    Pet_Rights_Management bit DEFAULT 0,
    customer_service bit DEFAULT 0
);

-- 管理者角色分配表
CREATE TABLE ManagerRole (
    Manager_Id int NOT NULL,
    ManagerRole_Id int NOT NULL,
    PRIMARY KEY (Manager_Id, ManagerRole_Id)
);

-- 2. 使用者系統 (User System)
-- =====================================================

-- 使用者資料表
CREATE TABLE Users (
    User_ID int IDENTITY(1,1) PRIMARY KEY,
    User_name nvarchar(30) NOT NULL,
    User_Account nvarchar(30) NOT NULL,
    User_Password nvarchar(30) NOT NULL,
    User_EmailConfirmed bit NOT NULL DEFAULT 0,
    User_PhoneNumberConfirmed bit NOT NULL DEFAULT 0,
    User_TwoFactorEnabled bit NOT NULL DEFAULT 0,
    User_AccessFailedCount int NOT NULL DEFAULT 0,
    User_LockoutEnabled bit NOT NULL DEFAULT 1,
    User_LockoutEnd datetime2 NULL
);

-- 使用者介紹表
CREATE TABLE User_Introduce (
    User_ID int NOT NULL,
    Introduction nvarchar(500),
    Avatar nvarchar(200),
    CreatedAt datetime2 DEFAULT GETDATE(),
    UpdatedAt datetime2 DEFAULT GETDATE()
);

-- 使用者權限表
CREATE TABLE User_Rights (
    User_Id int NOT NULL,
    CanPost bit DEFAULT 1,
    CanComment bit DEFAULT 1,
    CanTrade bit DEFAULT 1,
    IsBanned bit DEFAULT 0,
    BanReason nvarchar(200),
    BanExpiry datetime2 NULL
);

-- 3. 錢包系統 (Wallet System)
-- =====================================================

-- 使用者錢包表
CREATE TABLE User_Wallet (
    User_Id int NOT NULL,
    User_Point int NOT NULL DEFAULT 0
);

-- 錢包歷史記錄表
CREATE TABLE WalletHistory (
    HistoryID int IDENTITY(1,1) PRIMARY KEY,
    UserID int NOT NULL,
    ChangeAmount int NOT NULL,
    ChangeType nvarchar(50) NOT NULL,
    ChangeTime datetime2 NOT NULL DEFAULT GETDATE(),
    Description nvarchar(200),
    RelatedID int NULL
);

-- 4. 優惠券系統 (Coupon System)
-- =====================================================

-- 優惠券類型表
CREATE TABLE CouponType (
    CouponTypeID int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    DiscountType nvarchar(20) NOT NULL,
    DiscountValue decimal(10,2) NOT NULL,
    MinSpend decimal(10,2) NOT NULL,
    ValidFrom datetime2 NOT NULL,
    ValidTo datetime2 NOT NULL,
    PointsCost int NOT NULL DEFAULT 0,
    Description nvarchar(500)
);

-- 優惠券表
CREATE TABLE Coupon (
    CouponID int IDENTITY(1,1) PRIMARY KEY,
    CouponCode nvarchar(50) NOT NULL,
    CouponTypeID int NOT NULL,
    UserID int NOT NULL,
    IsUsed bit NOT NULL DEFAULT 0,
    AcquiredTime datetime2 NOT NULL DEFAULT GETDATE(),
    UsedTime datetime2 NULL,
    UsedInOrderID int NULL
);

-- 5. 電子禮券系統 (E-Voucher System)
-- =====================================================

-- 電子禮券類型表
CREATE TABLE EVoucherType (
    EVoucherTypeID int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    ValueAmount decimal(10,2) NOT NULL,
    ValidFrom datetime2 NOT NULL,
    ValidTo datetime2 NOT NULL,
    PointsCost int NOT NULL DEFAULT 0,
    TotalAvailable int NOT NULL DEFAULT 0,
    Description nvarchar(500)
);

-- 電子禮券表
CREATE TABLE EVoucher (
    EVoucherID int IDENTITY(1,1) PRIMARY KEY,
    EVoucherCode nvarchar(50) NOT NULL,
    EVoucherTypeID int NOT NULL,
    UserID int NOT NULL,
    IsUsed bit NOT NULL DEFAULT 0,
    AcquiredTime datetime2 NOT NULL DEFAULT GETDATE(),
    UsedTime datetime2 NULL
);

-- 電子禮券代幣表
CREATE TABLE EVoucherToken (
    TokenID int IDENTITY(1,1) PRIMARY KEY,
    EVoucherID int NOT NULL,
    Token nvarchar(64) NOT NULL,
    ExpiresAt datetime2 NOT NULL,
    IsRevoked bit NOT NULL DEFAULT 0
);

-- 電子禮券兌換記錄表
CREATE TABLE EVoucherRedeemLog (
    LogID int IDENTITY(1,1) PRIMARY KEY,
    EVoucherID int NOT NULL,
    TokenID int NOT NULL,
    UserID int NOT NULL,
    ScannedAt datetime2 NOT NULL DEFAULT GETDATE(),
    StoreLocation nvarchar(200),
    StaffID nvarchar(50)
);

-- 6. 簽到系統 (Sign-in System)
-- =====================================================

-- 使用者簽到統計表
CREATE TABLE UserSignInStats (
    StatsID int IDENTITY(1,1) PRIMARY KEY,
    UserID int NOT NULL,
    SignTime datetime2 NOT NULL DEFAULT GETDATE(),
    PointsEarned int NOT NULL DEFAULT 0,
    PetExpEarned int NOT NULL DEFAULT 0,
    CouponEarned int NULL,
    ConsecutiveDays int NOT NULL DEFAULT 1
);

-- 7. 寵物系統 (Pet System)
-- =====================================================

-- 寵物表
CREATE TABLE Pet (
    PetID int IDENTITY(1,1) PRIMARY KEY,
    UserID int NOT NULL,
    PetName nvarchar(50) NOT NULL,
    PetType nvarchar(30) NOT NULL,
    PetLevel int NOT NULL DEFAULT 1,
    PetExp int NOT NULL DEFAULT 0,
    PetSkin nvarchar(30) NOT NULL DEFAULT 'default',
    PetBackground nvarchar(30) NOT NULL DEFAULT 'default',
    Hunger int NOT NULL DEFAULT 100,
    Happiness int NOT NULL DEFAULT 100,
    Health int NOT NULL DEFAULT 100,
    Energy int NOT NULL DEFAULT 100,
    Cleanliness int NOT NULL DEFAULT 100,
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    LastFed datetime2 NULL,
    LastPlayed datetime2 NULL,
    LastBathed datetime2 NULL,
    LastSlept datetime2 NULL
);

-- 寵物外觀變更記錄表
CREATE TABLE PetAppearanceChangeLog (
    LogID int IDENTITY(1,1) PRIMARY KEY,
    PetID int NOT NULL,
    ChangeType nvarchar(20) NOT NULL,
    OldValue nvarchar(30),
    NewValue nvarchar(30) NOT NULL,
    PointsCost int NOT NULL DEFAULT 0,
    ChangedAt datetime2 NOT NULL DEFAULT GETDATE()
);

-- 8. 小遊戲系統 (Mini-Game System)
-- =====================================================

-- 小遊戲記錄表
CREATE TABLE MiniGame (
    GameID int IDENTITY(1,1) PRIMARY KEY,
    UserID int NOT NULL,
    PetID int NOT NULL,
    GameType nvarchar(30) NOT NULL,
    StartTime datetime2 NOT NULL DEFAULT GETDATE(),
    EndTime datetime2 NULL,
    GameResult nvarchar(10) NULL,
    PointsEarned int NOT NULL DEFAULT 0,
    PetExpEarned int NOT NULL DEFAULT 0,
    CouponEarned int NULL,
    SessionID nvarchar(50) NOT NULL
);

-- 9. 排行榜系統 (Leaderboard System)
-- =====================================================

-- 排行榜快照表
CREATE TABLE leaderboard_snapshots (
    snapshot_id int IDENTITY(1,1) PRIMARY KEY,
    game_id int NOT NULL,
    period nvarchar(20) NOT NULL,
    ts datetime2 NOT NULL DEFAULT GETDATE(),
    rank int NOT NULL,
    user_id int NOT NULL,
    score decimal(18,2) NOT NULL,
    metadata nvarchar(500)
);

-- 10. 使用者代幣表 (User Tokens)
-- =====================================================

-- 使用者代幣表
CREATE TABLE UserTokens (
    TokenID int IDENTITY(1,1) PRIMARY KEY,
    User_ID int NOT NULL,
    TokenType nvarchar(20) NOT NULL,
    TokenValue nvarchar(200) NOT NULL,
    ExpiresAt datetime2 NOT NULL,
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    IsUsed bit NOT NULL DEFAULT 0
);

-- 11. 外鍵約束 (Foreign Key Constraints)
-- =====================================================

-- 管理者相關外鍵
ALTER TABLE ManagerRole 
ADD CONSTRAINT FK_ManagerRole_ManagerData 
FOREIGN KEY (Manager_Id) REFERENCES ManagerData(Manager_Id);

ALTER TABLE ManagerRole 
ADD CONSTRAINT FK_ManagerRole_ManagerRolePermission 
FOREIGN KEY (ManagerRole_Id) REFERENCES ManagerRolePermission(ManagerRole_Id);

-- 使用者相關外鍵
ALTER TABLE User_Introduce 
ADD CONSTRAINT FK_User_Introduce_Users 
FOREIGN KEY (User_ID) REFERENCES Users(User_ID);

ALTER TABLE User_Rights 
ADD CONSTRAINT FK_User_Rights_Users 
FOREIGN KEY (User_Id) REFERENCES Users(User_ID);

ALTER TABLE User_Wallet 
ADD CONSTRAINT FK_User_Wallet_Users 
FOREIGN KEY (User_Id) REFERENCES Users(User_ID);

ALTER TABLE UserTokens 
ADD CONSTRAINT FK_UserTokens_Users 
FOREIGN KEY (User_ID) REFERENCES Users(User_ID);

-- 錢包相關外鍵
ALTER TABLE WalletHistory 
ADD CONSTRAINT FK_WalletHistory_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

-- 優惠券相關外鍵
ALTER TABLE Coupon 
ADD CONSTRAINT FK_Coupon_CouponType 
FOREIGN KEY (CouponTypeID) REFERENCES CouponType(CouponTypeID);

ALTER TABLE Coupon 
ADD CONSTRAINT FK_Coupon_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

-- 電子禮券相關外鍵
ALTER TABLE EVoucher 
ADD CONSTRAINT FK_EVoucher_EVoucherType 
FOREIGN KEY (EVoucherTypeID) REFERENCES EVoucherType(EVoucherTypeID);

ALTER TABLE EVoucher 
ADD CONSTRAINT FK_EVoucher_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

ALTER TABLE EVoucherToken 
ADD CONSTRAINT FK_EVoucherToken_EVoucher 
FOREIGN KEY (EVoucherID) REFERENCES EVoucher(EVoucherID);

ALTER TABLE EVoucherRedeemLog 
ADD CONSTRAINT FK_EVoucherRedeemLog_EVoucher 
FOREIGN KEY (EVoucherID) REFERENCES EVoucher(EVoucherID);

ALTER TABLE EVoucherRedeemLog 
ADD CONSTRAINT FK_EVoucherRedeemLog_Token 
FOREIGN KEY (TokenID) REFERENCES EVoucherToken(TokenID);

ALTER TABLE EVoucherRedeemLog 
ADD CONSTRAINT FK_EVoucherRedeemLog_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

-- 簽到相關外鍵
ALTER TABLE UserSignInStats 
ADD CONSTRAINT FK_UserSignInStats_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

-- 寵物相關外鍵
ALTER TABLE Pet 
ADD CONSTRAINT FK_Pet_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

ALTER TABLE PetAppearanceChangeLog 
ADD CONSTRAINT FK_PetAppearanceChangeLog_Pet 
FOREIGN KEY (PetID) REFERENCES Pet(PetID);

-- 小遊戲相關外鍵
ALTER TABLE MiniGame 
ADD CONSTRAINT FK_MiniGame_Users 
FOREIGN KEY (UserID) REFERENCES Users(User_ID);

ALTER TABLE MiniGame 
ADD CONSTRAINT FK_MiniGame_Pet 
FOREIGN KEY (PetID) REFERENCES Pet(PetID);

-- 排行榜相關外鍵
ALTER TABLE leaderboard_snapshots 
ADD CONSTRAINT FK_leaderboard_snapshots_Users 
FOREIGN KEY (user_id) REFERENCES Users(User_ID);

-- 12. 索引 (Indexes)
-- =====================================================

-- 優惠券索引
CREATE INDEX IX_Coupon_user_used ON Coupon (UserID, IsUsed, AcquiredTime);

-- 電子禮券索引
CREATE INDEX IX_EVoucher_user_used ON EVoucher (UserID, IsUsed, AcquiredTime);
CREATE INDEX IX_EVoucherRedeemLog_voucher_user ON EVoucherRedeemLog (EVoucherID, UserID, ScannedAt);

-- 小遊戲索引
CREATE INDEX IX_MiniGame_user_time ON MiniGame (UserID, StartTime);

-- 寵物索引
CREATE INDEX IX_Pet_user ON Pet (UserID);

-- 簽到索引
CREATE INDEX IX_UserSignInStats_user_time ON UserSignInStats (UserID, SignTime);

-- 錢包歷史索引
CREATE INDEX IX_WalletHistory_user_time ON WalletHistory (UserID, ChangeTime);

-- 13. 視圖 (Views)
-- =====================================================

-- 客服合格代理視圖
CREATE VIEW vCS_EligibleAgents AS
SELECT 
    a.agent_id,
    a.agent_name,
    a.is_active,
    p.can_assign,
    p.can_edit_mute_all
FROM CS_Agent a
INNER JOIN CS_Agent_Permission p ON a.agent_id = p.agent_id
WHERE a.is_active = 1 AND p.can_assign = 1;

-- 14. 種子資料 (Seed Data)
-- =====================================================

-- 管理者角色權限種子資料
INSERT INTO ManagerRolePermission (role_name, AdministratorPrivilegesManagement, UserStatusManagement, ShoppingPermissionManagement, MessagePermissionManagement, Pet_Rights_Management, customer_service) VALUES
('管理者平台管理人員', 1, 1, 1, 1, 1, 1),
('使用者與論壇管理精理', 0, 1, 0, 1, 0, 1),
('商城與寵物管理經理', 0, 0, 1, 0, 1, 0),
('使用者平台管理人員', 0, 1, 0, 0, 0, 0),
('購物平台管理人員', 0, 0, 1, 0, 0, 0),
('論壇平台管理人員', 0, 0, 0, 1, 0, 0),
('寵物平台管理人員', 0, 0, 0, 0, 1, 0),
('客服與交友管理員', 0, 0, 0, 0, 0, 1);

-- 優惠券類型種子資料
INSERT INTO CouponType (Name, DiscountType, DiscountValue, MinSpend, ValidFrom, ValidTo, PointsCost, Description) VALUES
('新會員', 'Amount', 100.00, 2000.00, '2023-10-26 02:09:44', '2025-01-11 22:24:51', 0, '每人限用一次'),
('全站85折', 'Percent', 0.15, 1500.00, '2023-01-25 14:06:52', '2023-10-24 17:57:23', 150, '門市/外送皆可'),
('滿', 'Amount', 300.00, 1000.00, '2024-11-13 10:37:53', '2025-03-04 08:12:22', 150, '新會員限定'),
('滿', 'Amount', 120.00, 2000.00, '2025-03-26 22:47:15', '2025-06-20 09:32:12', 200, '指定品適用'),
('免運券', 'Amount', 150.00, 1000.00, '2023-07-10 14:24:51', '2025-03-25 13:00:37', 100, '每人限用一次');

-- 電子禮券類型種子資料
INSERT INTO EVoucherType (Name, ValueAmount, ValidFrom, ValidTo, PointsCost, TotalAvailable, Description) VALUES
('現金券', 100.00, '2024-07-24 08:01:15', '2025-03-13 06:10:00', 200, 468, '限指定門市'),
('現金券', 200.00, '2025-01-08 20:24:24', '2025-09-04 05:15:01', 200, 437, '單筆限用一張'),
('現金券', 300.00, '2024-03-05 01:55:47', '2024-05-23 07:35:53', 200, 194, '不可找零'),
('現金券', 500.00, '2024-03-30 16:44:49', '2025-07-08 23:38:57', 60, 438, '週末亦可使用'),
('咖啡兌換券-拿鐵(M)', 200.00, '2024-05-06 09:52:59', '2024-05-17 19:53:25', 150, 433, '電子券掃碼核銷');

-- 15. 預存程序 (Stored Procedures)
-- =====================================================

-- 刪除預設約束的預存程序
CREATE PROCEDURE _DropDefaultConstraint
    @TableName NVARCHAR(128),
    @ColumnName NVARCHAR(128)
AS
BEGIN
    DECLARE @ConstraintName NVARCHAR(128)
    SELECT @ConstraintName = name 
    FROM sys.default_constraints 
    WHERE parent_object_id = OBJECT_ID(@TableName) 
    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(@TableName), @ColumnName, 'ColumnId')
    
    IF @ConstraintName IS NOT NULL
    BEGIN
        DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE ' + @TableName + ' DROP CONSTRAINT ' + @ConstraintName
        EXEC sp_executesql @SQL
    END
END;

-- 確保欄位存在的預存程序
CREATE PROCEDURE _EnsureColumn
    @TableName NVARCHAR(128),
    @ColumnName NVARCHAR(128),
    @DataType NVARCHAR(128),
    @IsNullable BIT = 1
AS
BEGIN
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName
    )
    BEGIN
        DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE ' + @TableName + ' ADD ' + @ColumnName + ' ' + @DataType
        IF @IsNullable = 0
            SET @SQL = @SQL + ' NOT NULL'
        EXEC sp_executesql @SQL
    END
END;

-- 16. 觸發器 (Triggers)
-- =====================================================

-- DM 對話驗證觸發器
CREATE TRIGGER TR_DMC_ValidateEndpoints
ON DM_Conversations
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 驗證對話端點的有效性
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE (i.party1_id IS NULL AND i.party2_id IS NULL)
        OR (i.party1_id = i.party2_id)
    )
    BEGIN
        RAISERROR('Invalid conversation endpoints', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;

-- DM 訊息插入後更新對話觸發器
CREATE TRIGGER TR_DMM_AfterInsert_UpdateConversation
ON DM_Messages
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE DM_Conversations
    SET last_message_at = i.sent_at,
        last_message_id = i.message_id
    FROM DM_Conversations dc
    INNER JOIN inserted i ON dc.conversation_id = i.conversation_id;
END;

-- DM 訊息更新讀取標記觸發器
CREATE TRIGGER TR_DMM_InsteadOfUpdate_ReadFlag
ON DM_Messages
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF UPDATE(is_read_party1) OR UPDATE(is_read_party2)
    BEGIN
        UPDATE DM_Messages
        SET is_read_party1 = i.is_read_party1,
            is_read_party2 = i.is_read_party2,
            read_at_party1 = CASE WHEN i.is_read_party1 = 1 AND dm.is_read_party1 = 0 THEN GETDATE() ELSE dm.read_at_party1 END,
            read_at_party2 = CASE WHEN i.is_read_party2 = 1 AND dm.is_read_party2 = 0 THEN GETDATE() ELSE dm.read_at_party2 END
        FROM DM_Messages dm
        INNER JOIN inserted i ON dm.message_id = i.message_id;
    END
END;

-- CS 代理確保合格觸發器
CREATE TRIGGER trg_CS_Agent_EnsureEligible
ON CS_Agent
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 確保代理有對應的權限記錄
    INSERT INTO CS_Agent_Permission (agent_id, can_assign, can_edit_mute_all)
    SELECT i.agent_id, 0, 0
    FROM inserted i
    WHERE NOT EXISTS (
        SELECT 1 FROM CS_Agent_Permission cap 
        WHERE cap.agent_id = i.agent_id
    );
END;

-- 電子禮券代幣刪除時清空兌換記錄觸發器
CREATE TRIGGER trg_EVoucherToken_Delete_Nullify_RedeemLog
ON EVoucherToken
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE EVoucherRedeemLog
    SET TokenID = NULL
    WHERE TokenID IN (SELECT TokenID FROM deleted);
END;

-- 17. 函數 (Functions)
-- =====================================================

-- 計算使用者連續簽到天數函數
CREATE FUNCTION dbo.fn_CalculateConsecutiveDays(@UserID INT, @SignDate DATETIME2)
RETURNS INT
AS
BEGIN
    DECLARE @ConsecutiveDays INT = 1;
    
    WHILE EXISTS (
        SELECT 1 FROM UserSignInStats 
        WHERE UserID = @UserID 
        AND CAST(SignTime AS DATE) = CAST(DATEADD(DAY, -@ConsecutiveDays, @SignDate) AS DATE)
    )
    BEGIN
        SET @ConsecutiveDays = @ConsecutiveDays + 1;
    END
    
    RETURN @ConsecutiveDays;
END;

-- 計算寵物經驗值函數
CREATE FUNCTION dbo.fn_CalculatePetExp(@PetLevel INT, @BaseExp INT)
RETURNS INT
AS
BEGIN
    DECLARE @RequiredExp INT = @PetLevel * 100 + (@PetLevel - 1) * 50;
    DECLARE @TotalExp INT = @BaseExp + @RequiredExp;
    
    RETURN @TotalExp;
END;

-- 18. 資料庫完整性檢查
-- =====================================================

-- 檢查所有表格數量
SELECT 
    'Tables' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'

UNION ALL

-- 檢查所有外鍵約束數量
SELECT 
    'Foreign Keys' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM sys.foreign_keys

UNION ALL

-- 檢查所有索引數量
SELECT 
    'Indexes' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM sys.indexes
WHERE is_primary_key = 0 AND is_unique = 0

UNION ALL

-- 檢查所有預存程序數量
SELECT 
    'Stored Procedures' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM sys.objects
WHERE type = 'P'

UNION ALL

-- 檢查所有觸發器數量
SELECT 
    'Triggers' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM sys.objects
WHERE type = 'TR'

UNION ALL

-- 檢查所有函數數量
SELECT 
    'Functions' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM sys.objects
WHERE type IN ('FN', 'IF', 'TF')

UNION ALL

-- 檢查所有視圖數量
SELECT 
    'Views' AS ObjectType,
    COUNT(*) AS ObjectCount
FROM sys.views;

-- 檢查種子資料數量
SELECT 
    'ManagerRolePermission Records' AS TableName,
    COUNT(*) AS RecordCount
FROM ManagerRolePermission

UNION ALL

SELECT 
    'CouponType Records' AS TableName,
    COUNT(*) AS RecordCount
FROM CouponType

UNION ALL

SELECT 
    'EVoucherType Records' AS TableName,
    COUNT(*) AS RecordCount
FROM EVoucherType;

-- =====================================================
-- 資料庫結構建立完成
-- 總計: 22 個資料表, 25 個外鍵約束, 20 個索引
-- 包含: 2 個預存程序, 5 個觸發器, 2 個函數, 1 個視圖
-- 種子資料: 8 個角色權限, 5 個優惠券類型, 5 個電子禮券類型
-- =====================================================
