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
    public class WalletAdminController : MiniGameBaseController
    {
        public WalletAdminController(GameSpacedatabaseContext context)
            : base(context)
        {
        }

        [HttpGet]
        public async Task<IActionResult> PointsQuery([FromQuery] WalletQueryModel query)
        {
            query ??= new WalletQueryModel();
            var page = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 10, 200);

            var source = _context.UserWallets
                .AsNoTracking()
                .Include(w => w.User)
                .AsQueryable();

            if (query.UserId.HasValue)
            {
                source = source.Where(w => w.UserId == query.UserId.Value);
            }

            if (query.MinAmount.HasValue)
            {
                source = source.Where(w => w.UserPoint >= query.MinAmount.Value);
            }

            if (query.MaxAmount.HasValue)
            {
                source = source.Where(w => w.UserPoint <= query.MaxAmount.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim();
                source = source.Where(w =>
                    (w.User != null && (w.User.UserAccount.Contains(term) || w.User.UserName.Contains(term) || w.User.User_email.Contains(term))));
            }

            source = query.SortBy?.ToLowerInvariant() switch
            {
                "points_asc" => source.OrderBy(w => w.UserPoint),
                "userid_desc" => source.OrderByDescending(w => w.UserId),
                "userid_asc" => source.OrderBy(w => w.UserId),
                _ => source.OrderByDescending(w => w.UserPoint)
            };

            var totalCount = await source.CountAsync();
            var items = await source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var records = items.Select(w => new WalletPointRecord
            {
                UserId = w.UserId,
                UserAccount = w.User?.UserAccount ?? string.Empty,
                UserName = w.User?.UserName ?? string.Empty,
                Email = w.User?.User_email ?? string.Empty,
                Points = w.UserPoint
            }).ToList();

            var model = new WalletPointsQueryViewModel
            {
                Query = query,
                Results = new PagedResult<WalletPointRecord>
                {
                    Items = records,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CouponsQuery([FromQuery] CouponQueryModel query)
        {
            query ??= new CouponQueryModel();
            var page = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 10, 200);

            var source = _context.Coupons
                .AsNoTracking()
                .Include(c => c.User)
                .Include(c => c.CouponType)
                .AsQueryable();

            if (query.UserId.HasValue)
            {
                source = source.Where(c => c.UserId == query.UserId.Value);
            }

            if (query.CouponTypeId.HasValue)
            {
                source = source.Where(c => c.CouponTypeId == query.CouponTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                source = query.Status.ToLowerInvariant() switch
                {
                    "used" => source.Where(c => c.IsUsed),
                    "unused" => source.Where(c => !c.IsUsed && (!c.CouponType.ValidTo.Equals(default) ? c.CouponType.ValidTo >= DateTime.UtcNow : true)),
                    "expired" => source.Where(c => !c.IsUsed && c.CouponType.ValidTo < DateTime.UtcNow),
                    _ => source
                };
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim();
                source = source.Where(c =>
                    c.CouponCode.Contains(term) ||
                    (c.User != null && (c.User.UserAccount.Contains(term) || c.User.UserName.Contains(term))));
            }

            source = query.SortBy?.ToLowerInvariant() switch
            {
                "acquiredtime" => query.Descending ? source.OrderByDescending(c => c.AcquiredTime) : source.OrderBy(c => c.AcquiredTime),
                "usetime" => query.Descending ? source.OrderByDescending(c => c.UsedTime) : source.OrderBy(c => c.UsedTime),
                _ => source.OrderByDescending(c => c.AcquiredTime)
            };

            var totalCount = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var records = items.Select(c => new UserCouponReadModel
            {
                CouponId = c.CouponId,
                CouponCode = c.CouponCode,
                UserId = c.UserId,
                UserName = c.User?.UserName ?? string.Empty,
                Email = c.User?.User_email ?? string.Empty,
                CouponTypeId = c.CouponTypeId,
                CouponTypeName = c.CouponType?.Name ?? string.Empty,
                DiscountAmount = c.CouponType?.DiscountValue ?? 0,
                DiscountPercentage = c.CouponType?.DiscountType == "Percentage" ? c.CouponType?.DiscountValue : null,
                MinimumPurchase = c.CouponType?.MinSpend,
                AcquiredTime = c.AcquiredTime,
                UsedTime = c.UsedTime,
                ExpiryDate = c.CouponType?.ValidTo,
                IsUsed = c.IsUsed
            }).ToList();

            var model = new WalletCouponsQueryViewModel
            {
                Query = query,
                Results = new PagedResult<UserCouponReadModel>
                {
                    Items = records,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EVouchersQuery([FromQuery] EVoucherQueryModel query)
        {
            query ??= new EVoucherQueryModel();
            var page = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 10, 200);

            var source = _context.Evouchers
                .AsNoTracking()
                .Include(e => e.User)
                .Include(e => e.EvoucherType)
                .AsQueryable();

            if (query.UserId.HasValue)
            {
                source = source.Where(e => e.UserId == query.UserId.Value);
            }

            if (query.EVoucherTypeId.HasValue)
            {
                source = source.Where(e => e.EvoucherTypeId == query.EVoucherTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.TypeCode))
            {
                source = source.Where(e => e.EvoucherType.Name.Contains(query.TypeCode));
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim();
                source = source.Where(e =>
                    e.EvoucherCode.Contains(term) ||
                    (e.User != null && (e.User.UserAccount.Contains(term) || e.User.UserName.Contains(term))));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                source = query.Status.ToLowerInvariant() switch
                {
                    "used" => source.Where(e => e.IsUsed),
                    "unused" => source.Where(e => !e.IsUsed && e.EvoucherType.ValidTo >= DateTime.UtcNow),
                    "expired" => source.Where(e => !e.IsUsed && e.EvoucherType.ValidTo < DateTime.UtcNow),
                    _ => source
                };
            }

            source = query.SortBy?.ToLowerInvariant() switch
            {
                "acquiredtime" => query.Descending ? source.OrderByDescending(e => e.AcquiredTime) : source.OrderBy(e => e.AcquiredTime),
                "validto" => query.Descending ? source.OrderByDescending(e => e.EvoucherType.ValidTo) : source.OrderBy(e => e.EvoucherType.ValidTo),
                _ => source.OrderByDescending(e => e.AcquiredTime)
            };

            var totalCount = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var records = items.Select(e => new EVoucherReadModel
            {
                EVoucherId = e.EvoucherId,
                EVoucherCode = e.EvoucherCode,
                UserId = e.UserId,
                UserName = e.User?.UserName ?? string.Empty,
                UserEmail = e.User?.User_email ?? string.Empty,
                EVoucherTypeId = e.EvoucherTypeId,
                EVoucherTypeName = e.EvoucherType?.Name ?? string.Empty,
                VoucherValue = e.EvoucherType?.ValueAmount ?? 0,
                MerchantName = e.EvoucherType?.Description,
                IsUsed = e.IsUsed,
                AcquiredTime = e.AcquiredTime,
                UsedTime = e.UsedTime,
                ValidFrom = e.EvoucherType?.ValidFrom ?? DateTime.MinValue,
                ValidTo = e.EvoucherType?.ValidTo ?? DateTime.MinValue,
                UsedLocation = null
            }).ToList();

            var model = new WalletEVouchersQueryViewModel
            {
                Query = query,
                Results = new PagedResult<EVoucherReadModel>
                {
                    Items = records,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> WalletHistory([FromQuery] WalletHistoryQueryModel query)
        {
            query ??= new WalletHistoryQueryModel();
            var page = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 10, 200);

            var source = _context.WalletHistories
                .AsNoTracking()
                .Include(h => h.User)
                .AsQueryable();

            if (query.UserId.HasValue)
            {
                source = source.Where(h => h.UserId == query.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.ChangeType))
            {
                source = source.Where(h => h.ChangeType == query.ChangeType);
            }

            if (query.StartDate.HasValue)
            {
                source = source.Where(h => h.ChangeTime >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                source = source.Where(h => h.ChangeTime <= query.EndDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim();
                source = source.Where(h =>
                    h.Description != null && h.Description.Contains(term) ||
                    (h.User != null && (h.User.UserAccount.Contains(term) || h.User.UserName.Contains(term))));
            }

            source = source.OrderByDescending(h => h.ChangeTime);

            var totalCount = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var lookupIds = items.Select(h => h.UserId).Distinct().ToList();
            var walletLookup = await _context.UserWallets
                .AsNoTracking()
                .Where(w => lookupIds.Contains(w.UserId))
                .ToDictionaryAsync(w => w.UserId, w => w.UserPoint);

            var records = items.Select(h => new WalletHistoryRecord
            {
                LogId = h.LogId,
                UserId = h.UserId,
                UserAccount = h.User?.UserAccount ?? string.Empty,
                UserName = h.User?.UserName ?? string.Empty,
                ChangeType = h.ChangeType,
                PointsChanged = h.PointsChanged,
                BalanceAfter = walletLookup.TryGetValue(h.UserId, out var balance) ? balance : 0,
                Description = h.Description,
                ChangeTime = h.ChangeTime
            }).ToList();

            var model = new WalletHistoryViewModel
            {
                Query = query,
                Results = new PagedResult<WalletHistoryRecord>
                {
                    Items = records,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GrantPoints()
        {
            var candidates = await _context.UserWallets
                .AsNoTracking()
                .Include(w => w.User)
                .OrderByDescending(w => w.UserPoint)
                .Take(20)
                .ToListAsync();

            var model = new WalletGrantPointsViewModel
            {
                Candidates = candidates.Select(w => new WalletPointRecord
                {
                    UserId = w.UserId,
                    UserAccount = w.User?.UserAccount ?? string.Empty,
                    UserName = w.User?.UserName ?? string.Empty,
                    Email = w.User?.User_email ?? string.Empty,
                    Points = w.UserPoint
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantPoints(int userId, int pointsToGrant, string reason)
        {
            if (pointsToGrant <= 0)
            {
                TempData["Error"] = "發放點數必須大於 0";
                return RedirectToAction(nameof(GrantPoints));
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "請填寫發放原因";
                return RedirectToAction(nameof(GrantPoints));
            }

            var wallet = await _context.UserWallets.FindAsync(userId);
            if (wallet == null)
            {
                TempData["Error"] = "找不到該會員的錢包";
                return RedirectToAction(nameof(GrantPoints));
            }

            // 更新會員點數
            wallet.UserPoint += pointsToGrant;

            // 記錄異動歷史
            var history = new WalletHistory
            {
                UserId = userId,
                ChangeType = "Point",
                PointsChanged = pointsToGrant,
                Description = reason,
                ChangeTime = DateTime.UtcNow
            };
            _context.WalletHistories.Add(history);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"成功發放 {pointsToGrant} 點數給會員 ID {userId}";
            return RedirectToAction(nameof(GrantPoints));
        }

        [HttpGet]
        public async Task<IActionResult> GrantCoupon()
        {
            var summaries = await _context.CouponTypes
                .AsNoTracking()
                .Select(ct => new CouponTypeSummary
                {
                    CouponTypeId = ct.CouponTypeId,
                    TypeName = ct.Name,
                    Description = ct.Description,
                    TotalIssued = ct.Coupons.Count,
                    UnusedCount = ct.Coupons.Count(c => !c.IsUsed)
                })
                .OrderBy(ct => ct.TypeName)
                .ToListAsync();

            var model = new WalletGrantCouponViewModel
            {
                CouponTypes = summaries
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantCoupon(int userId, int couponTypeId)
        {
            // 驗證會員存在
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                TempData["Error"] = $"找不到會員 ID {userId}";
                return RedirectToAction(nameof(GrantCoupon));
            }

            // 驗證優惠券類型存在
            var couponType = await _context.CouponTypes.FindAsync(couponTypeId);
            if (couponType == null)
            {
                TempData["Error"] = "找不到該優惠券類型";
                return RedirectToAction(nameof(GrantCoupon));
            }

            // 生成優惠券序號 CPN-YYYYMM-XXXXXX
            var now = DateTime.UtcNow;
            var random = new Random();
            var randomCode = new string(Enumerable.Range(0, 6)
                .Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[random.Next(36)])
                .ToArray());
            var couponCode = $"CPN-{now:yyMM}-{randomCode}";

            // 創建優惠券
            var coupon = new Coupon
            {
                CouponCode = couponCode,
                CouponTypeId = couponTypeId,
                UserId = userId,
                IsUsed = false,
                AcquiredTime = now
            };
            _context.Coupons.Add(coupon);

            // 記錄異動歷史
            var history = new WalletHistory
            {
                UserId = userId,
                ChangeType = "Coupon",
                PointsChanged = 0,
                ItemCode = couponCode,
                Description = $"發放商城優惠券：{couponType.Name}",
                ChangeTime = now
            };
            _context.WalletHistories.Add(history);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"成功發放優惠券「{couponType.Name}」給會員 ID {userId}，序號：{couponCode}";
            return RedirectToAction(nameof(GrantCoupon));
        }

        [HttpGet]
        public async Task<IActionResult> AdjustEVoucher()
        {
            var summaries = await _context.EvoucherTypes
                .AsNoTracking()
                .Select(et => new EVoucherTypeSummary
                {
                    EVoucherTypeId = et.EvoucherTypeId,
                    TypeName = et.Name,
                    MerchantName = et.Description,
                    VoucherValue = et.ValueAmount,
                    TotalIssued = et.Evouchers.Count,
                    UnusedCount = et.Evouchers.Count(ev => !ev.IsUsed)
                })
                .OrderBy(et => et.TypeName)
                .ToListAsync();

            var model = new WalletAdjustEVoucherViewModel
            {
                EVoucherTypes = summaries
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustEVoucher(int userId, int evoucherTypeId, string action)
        {
            // 驗證會員存在
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                TempData["Error"] = $"找不到會員 ID {userId}";
                return RedirectToAction(nameof(AdjustEVoucher));
            }

            // 驗證電子禮券類型存在
            var evoucherType = await _context.EvoucherTypes.FindAsync(evoucherTypeId);
            if (evoucherType == null)
            {
                TempData["Error"] = "找不到該電子禮券類型";
                return RedirectToAction(nameof(AdjustEVoucher));
            }

            var now = DateTime.UtcNow;

            if (action == "grant")
            {
                // 發放電子禮券 - 生成序號 EV-{類型}-{4位隨機碼}-{6位數字}
                var random = new Random();
                var randomCode = new string(Enumerable.Range(0, 4)
                    .Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(26)])
                    .ToArray());
                var randomDigits = random.Next(0, 1000000).ToString("D6");

                // 從類型名稱提取類型代碼
                string typeCode = evoucherType.Name.Contains("現金") ? "CASH" :
                                  evoucherType.Name.Contains("電影") ? "MOVIE" :
                                  evoucherType.Name.Contains("餐") ? "FOOD" :
                                  evoucherType.Name.Contains("加油") ? "GAS" :
                                  evoucherType.Name.Contains("咖啡") ? "COFFEE" : "STORE";

                var evoucherCode = $"EV-{typeCode}-{randomCode}-{randomDigits}";

                // 創建電子禮券
                var evoucher = new Evoucher
                {
                    EvoucherCode = evoucherCode,
                    EvoucherTypeId = evoucherTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = now
                };
                _context.Evouchers.Add(evoucher);

                // 記錄異動歷史
                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "EVoucher",
                    PointsChanged = 0,
                    ItemCode = evoucherCode,
                    Description = $"發放電子禮券：{evoucherType.Name}",
                    ChangeTime = now
                };
                _context.WalletHistories.Add(history);

                await _context.SaveChangesAsync();

                TempData["Success"] = $"成功發放電子禮券「{evoucherType.Name}」給會員 ID {userId}，序號：{evoucherCode}";
            }
            else if (action == "revoke")
            {
                // 撤銷電子禮券 - 找到該會員最近一張未使用的該類型禮券
                var evoucherToRevoke = await _context.Evouchers
                    .Where(ev => ev.UserId == userId && ev.EvoucherTypeId == evoucherTypeId && !ev.IsUsed)
                    .OrderByDescending(ev => ev.AcquiredTime)
                    .FirstOrDefaultAsync();

                if (evoucherToRevoke == null)
                {
                    TempData["Error"] = "找不到可撤銷的電子禮券";
                    return RedirectToAction(nameof(AdjustEVoucher));
                }

                _context.Evouchers.Remove(evoucherToRevoke);

                // 記錄異動歷史
                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "EVoucher",
                    PointsChanged = 0,
                    ItemCode = evoucherToRevoke.EvoucherCode,
                    Description = $"撤銷電子禮券：{evoucherType.Name}",
                    ChangeTime = now
                };
                _context.WalletHistories.Add(history);

                await _context.SaveChangesAsync();

                TempData["Success"] = $"成功撤銷會員 ID {userId} 的電子禮券「{evoucherType.Name}」，序號：{evoucherToRevoke.EvoucherCode}";
            }
            else
            {
                TempData["Error"] = "無效的操作";
            }

            return RedirectToAction(nameof(AdjustEVoucher));
        }
    }
}
