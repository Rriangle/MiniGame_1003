using System;
using System.Collections.Generic;

namespace GameSpace.Areas.MiniGame.Models.ViewModels
{
    public class WalletPointsQueryViewModel
    {
        public WalletQueryModel Query { get; set; } = new();
        public PagedResult<WalletPointRecord> Results { get; set; } = new();
    }

    public class WalletPointRecord
    {
        public int UserId { get; set; }
        public string UserAccount { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Points { get; set; }
    }

    public class WalletCouponsQueryViewModel
    {
        public CouponQueryModel Query { get; set; } = new();
        public PagedResult<UserCouponReadModel> Results { get; set; } = new();
    }

    public class WalletEVouchersQueryViewModel
    {
        public EVoucherQueryModel Query { get; set; } = new();
        public PagedResult<EVoucherReadModel> Results { get; set; } = new();
    }

    public class WalletHistoryQueryModel
    {
        public int? UserId { get; set; }
        public string? ChangeType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "ChangeTime";
        public bool Descending { get; set; } = true;
    }

    public class WalletHistoryRecord
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string UserAccount { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public int PointsChanged { get; set; }
        public int BalanceAfter { get; set; }
        public string? Description { get; set; }
        public DateTime ChangeTime { get; set; }
    }

    public class WalletHistoryViewModel
    {
        public WalletHistoryQueryModel Query { get; set; } = new();
        public PagedResult<WalletHistoryRecord> Results { get; set; } = new();
    }

    public class CouponTypeSummary
    {
        public int CouponTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalIssued { get; set; }
        public int UnusedCount { get; set; }
    }

    public class EVoucherTypeSummary
    {
        public int EVoucherTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? MerchantName { get; set; }
        public decimal VoucherValue { get; set; }
        public int TotalIssued { get; set; }
        public int UnusedCount { get; set; }
    }

    public class WalletGrantPointsViewModel
    {
        public IReadOnlyList<WalletPointRecord> Candidates { get; set; } = Array.Empty<WalletPointRecord>();
    }

    public class WalletGrantCouponViewModel
    {
        public IReadOnlyList<CouponTypeSummary> CouponTypes { get; set; } = Array.Empty<CouponTypeSummary>();
    }

    public class WalletAdjustEVoucherViewModel
    {
        public IReadOnlyList<EVoucherTypeSummary> EVoucherTypes { get; set; } = Array.Empty<EVoucherTypeSummary>();
    }

    public class SignInRuleDisplay
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public int Points { get; set; }
        public int Experience { get; set; }
        public bool HasCoupon { get; set; }
        public string? CouponTypeCode { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class SignInRuleSettingsViewModel
    {
        public IReadOnlyList<SignInRuleDisplay> Rules { get; set; } = Array.Empty<SignInRuleDisplay>();
        public int ActiveRuleCount => Rules.Count(r => r.IsActive);
    }

    public class SignInRecordViewModel
    {
        public int Id { get; set; }
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string UserAccount { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime SignTime { get; set; }
        public DateTime SignInDate { get; set; }
        public int ConsecutiveDays { get; set; }
        public int PointsGained { get; set; }
        public int PointsEarned { get; set; }
        public int ExpGained { get; set; }
        public string? CouponCode { get; set; }
        public string? BonusType { get; set; }
        public string? IPAddress { get; set; }
    }

    public class SignInRecordsViewModel
    {
        public PagedResult<SignInRecordViewModel> Records { get; set; } = new();
    }

    public class PetMemberSummaryViewModel
    {
        public int PetId { get; set; }
        public int UserId { get; set; }
        public string UserAccount { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PetName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public string SkinColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public int Health { get; set; }
        public int Hunger { get; set; }
        public int Mood { get; set; }
        public int Stamina { get; set; }
        public int Cleanliness { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PetMemberListViewModel
    {
        public PagedResult<PetMemberSummaryViewModel> Pets { get; set; } = new();
    }

    public class PetMemberEditViewModel
    {
        public PetMemberSummaryViewModel? Pet { get; set; }
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.PetColorOption> SkinOptions { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.PetColorOption>();
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.PetBackgroundOptionEntity> BackgroundOptions { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.PetBackgroundOptionEntity>();
    }

    public class PetChangeRecordViewModel
    {
        public int PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FromValue { get; set; } = string.Empty;
        public string ToValue { get; set; } = string.Empty;
        public int PointCost { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public class PetChangeLogViewModel
    {
        public string Title { get; set; } = string.Empty;
        public PagedResult<PetChangeRecordViewModel> Records { get; set; } = new();
    }

    public class PetRuleSettingsViewModel
    {
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.ViewModels.PetRuleReadModel> LevelRules { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.ViewModels.PetRuleReadModel>();
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.ViewModels.PetInteractionBonusRules> InteractionBonusRules { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.ViewModels.PetInteractionBonusRules>();
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.PetColorOption> SkinOptions { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.PetColorOption>();
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.PetBackgroundOptionEntity> BackgroundOptions { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.PetBackgroundOptionEntity>();
        public IReadOnlyList<GameSpace.Areas.MiniGame.Models.Settings.PetSkinColorPointSettings> SkinPointSettings { get; set; } = Array.Empty<GameSpace.Areas.MiniGame.Models.Settings.PetSkinColorPointSettings>();
    }

    public class GameRuleSummary
    {
        public int Id { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty;
        public string RuleValue { get; set; } = string.Empty;
    }

    public class GameEventRuleSummary
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public decimal RewardMultiplier { get; set; }
    }

    public class GameRuleSettingsViewModel
    {
        public int DailyGameLimit { get; set; }
        public IReadOnlyList<GameRuleSummary> RewardRules { get; set; } = Array.Empty<GameRuleSummary>();
        public IReadOnlyList<GameEventRuleSummary> EventRules { get; set; } = Array.Empty<GameEventRuleSummary>();
    }

    public class GameRecordViewModel
    {
        public int Id { get; set; }
        public int PlayId { get; set; }
        public int UserId { get; set; }
        public string UserAccount { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Result { get; set; } = string.Empty;
        public int Score { get; set; }
        public int PointsGained { get; set; }
        public int PointsEarned { get; set; }
        public int ExpGained { get; set; }
        public string? CouponGained { get; set; }
        public bool Aborted { get; set; }
        public int Duration { get; set; }
        public int Level { get; set; }
    }

    public class GameRecordsViewModel
    {
        public PagedResult<GameRecordViewModel> Records { get; set; } = new();
    }
}
