using GameSpace.Areas.MiniGame.Models.ViewModels;
using GameSpace.Areas.social_hub.Auth;
using GameSpace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Controllers
{
    [Area("MiniGame")]
    [Authorize(AuthenticationSchemes = AuthConstants.AdminCookieScheme, Policy = "AdminOnly")]
    [Route("Admin/MiniGame/[controller]/[action]")]
    public class SignInAdminController : MiniGameBaseController
    {
        public SignInAdminController(GameSpacedatabaseContext context)
            : base(context)
        {
        }

        [HttpGet]
        public async Task<IActionResult> RuleSettings()
        {
            var rules = await _context.SignInRules
                .AsNoTracking()
                .OrderBy(r => r.SignInDay)
                .Select(r => new SignInRuleDisplay
                {
                    Id = r.Id,
                    DayNumber = r.SignInDay,
                    Points = r.Points,
                    Experience = r.Experience,
                    HasCoupon = r.HasCoupon,
                    CouponTypeCode = r.CouponTypeCode,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            var model = new SignInRuleSettingsViewModel
            {
                Rules = rules
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRule(int ruleId, int points, int experience, bool hasCoupon, string? couponTypeCode, bool isActive)
        {
            if (points < 0 || experience < 0)
            {
                TempData["Error"] = "點數和經驗值不能為負數";
                return RedirectToAction(nameof(RuleSettings));
            }

            var rule = await _context.SignInRules.FindAsync(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "找不到該簽到規則";
                return RedirectToAction(nameof(RuleSettings));
            }

            // 更新規則
            rule.Points = points;
            rule.Experience = experience;
            rule.HasCoupon = hasCoupon;
            rule.CouponTypeCode = hasCoupon ? couponTypeCode : null;
            rule.IsActive = isActive;
            rule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"成功更新第 {rule.SignInDay} 天的簽到規則";
            return RedirectToAction(nameof(RuleSettings));
        }

        [HttpGet]
        public async Task<IActionResult> Records(int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);

            var source = _context.UserSignInStats
                .AsNoTracking()
                .Include(s => s.User)
                .OrderByDescending(s => s.SignTime);

            var totalCount = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var records = items.Select(s => new SignInRecordViewModel
            {
                LogId = s.LogId,
                UserId = s.UserId,
                UserAccount = s.User?.UserAccount ?? string.Empty,
                UserName = s.User?.UserName ?? string.Empty,
                SignTime = s.SignTime,
                PointsGained = s.PointsGained,
                ExpGained = s.ExpGained,
                CouponCode = string.IsNullOrWhiteSpace(s.CouponGained) ? null : s.CouponGained
            }).ToList();

            var model = new SignInRecordsViewModel
            {
                Records = new PagedResult<SignInRecordViewModel>
                {
                    Items = records,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

            return View(model);
        }
    }
}
