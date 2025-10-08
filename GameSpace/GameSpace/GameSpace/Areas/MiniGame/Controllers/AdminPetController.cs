using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GameSpace.Areas.MiniGame.Models;
using GameSpace.Areas.MiniGame.Models.ViewModels;
using GameSpace.Areas.MiniGame.Models.Settings;
using GameSpace.Areas.MiniGame.Services;
using GameSpace.Areas.social_hub.Auth;
using GameSpace.Models;

namespace GameSpace.Areas.MiniGame.Controllers
{
    [Area("MiniGame")]
    [Authorize(AuthenticationSchemes = AuthConstants.AdminCookieScheme)]
    public class AdminPetController : MiniGameBaseController
    {
        private readonly IPetService _petService;
        private readonly IPetRulesService _petRulesService;

        public AdminPetController(
            GameSpacedatabaseContext context,
            IPetService petService,
            IPetRulesService petRulesService)
            : base(context)
        {
            _petService = petService;
            _petRulesService = petRulesService;
        }

        // GET: AdminPet
        // Note: Pet entity only has basic properties (PetName, Level, Experience, etc.), not PetType/Description/Rarity
        public async Task<IActionResult> Index(string searchTerm = "", string sortBy = "name", int page = 1, int pageSize = 10)
        {
            var pets = await _petService.GetAllPetsAsync();

            // 搜尋功能
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (int.TryParse(searchTerm, out int userId))
                {
                    pets = pets.Where(p => p.UserId == userId);
                }
                else
                {
                    pets = pets.Where(p => p.PetName.Contains(searchTerm));
                }
            }

            // 排序
            pets = sortBy switch
            {
                "level" => pets.OrderByDescending(p => p.Level),
                "exp" => pets.OrderByDescending(p => p.Experience),
                "health" => pets.OrderByDescending(p => p.Health),
                _ => pets.OrderBy(p => p.PetName)
            };

            // 分頁
            var totalCount = pets.Count();
            var pagedPets = pets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new AdminPetIndexViewModel
            {
                Pets = new PagedResult<GameSpace.Models.Pet>
                {
                    Items = pagedPets,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                }
            };

            // 設定 ViewBag 用於搜尋和篩選
            var allPets = await _petService.GetAllPetsAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.TotalPets = allPets.Count();
            ViewBag.HighLevelPets = allPets.Count(p => p.Level >= 10);
            ViewBag.HealthyPets = allPets.Count(p => p.Health >= 80);
            ViewBag.AverageLevel = allPets.Any() ? allPets.Average(p => (double)p.Level) : 0;
            ViewBag.AverageHealth = allPets.Any() ? allPets.Average(p => (double)p.Health) : 0;

            return View(viewModel);
        }

        // GET: AdminPet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _petService.GetPetByIdAsync(id.Value);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: AdminPet/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminPet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminPetCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pet = new GameSpace.Models.Pet
                {
                    UserId = model.UserId,
                    PetName = model.PetName,
                    SkinColor = model.SkinColor,
                    SkinColorChangedTime = DateTime.Now,
                    BackgroundColor = model.BackgroundColor,
                    BackgroundColorChangedTime = DateTime.Now,
                    // 初始化寵物狀態
                    Level = 1,
                    LevelUpTime = DateTime.Now,
                    Experience = 0,
                    Hunger = 100,
                    Mood = 100,
                    Stamina = 100,
                    Cleanliness = 100,
                    Health = 100,
                    PointsChangedSkinColor = 0,
                    PointsChangedBackgroundColor = 0,
                    PointsGainedLevelUp = 0,
                    PointsGainedTimeLevelUp = DateTime.Now
                };

                var result = await _petService.CreatePetAsync(pet);

                if (result)
                {
                    TempData["SuccessMessage"] = "寵物建立成功";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "建立寵物失敗");
                }
            }

            return View(model);
        }

        // GET: AdminPet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _petService.GetPetByIdAsync(id.Value);
            if (pet == null)
            {
                return NotFound();
            }

            var model = new AdminPetCreateViewModel
            {
                UserId = pet.UserId,
                PetName = pet.PetName,
                SkinColor = pet.SkinColor,
                BackgroundColor = pet.BackgroundColor
            };

            return View(model);
        }

        // POST: AdminPet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminPetCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pet = await _petService.GetPetByIdAsync(id);
                if (pet == null)
                {
                    return NotFound();
                }

                pet.UserId = model.UserId;
                pet.PetName = model.PetName;
                pet.SkinColor = model.SkinColor;
                pet.BackgroundColor = model.BackgroundColor;

                var result = await _petService.UpdatePetAsync(pet);

                if (result)
                {
                    TempData["SuccessMessage"] = "寵物更新成功";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "更新寵物失敗");
                }
            }

            return View(model);
        }

        // GET: AdminPet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _petService.GetPetByIdAsync(id.Value);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // POST: AdminPet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _petService.DeletePetAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "寵物刪除成功";
            }
            else
            {
                TempData["ErrorMessage"] = "寵物刪除失敗";
            }

            return RedirectToAction(nameof(Index));
        }

        // 切換寵物狀態 - Pet entity doesn't have IsActive property, removed this functionality
        // [HttpPost]
        // public async Task<IActionResult> ToggleStatus(int id)
        // Note: Pet entity doesn't have IsActive property - this method is disabled

        // 獲取寵物統計數據
        [HttpGet]
        public async Task<IActionResult> GetPetStats()
        {
            var allPets = await _petService.GetAllPetsAsync();
            var stats = new
            {
                total = allPets.Count(),
                highLevel = allPets.Count(p => p.Level >= 10),
                healthy = allPets.Count(p => p.Health >= 80),
                hungry = allPets.Count(p => p.Hunger < 30),
                avgLevel = allPets.Any() ? allPets.Average(p => (double)p.Level) : 0,
                avgHealth = allPets.Any() ? allPets.Average(p => (double)p.Health) : 0
            };

            return Json(stats);
        }

        // 獲取寵物等級分佈
        [HttpGet]
        public async Task<IActionResult> GetPetRarityDistribution()
        {
            var allPets = await _petService.GetAllPetsAsync();

            var distribution = allPets
                .GroupBy(p => p.Level / 10 * 10) // Group by 10-level ranges (0-9, 10-19, etc.)
                .Select(g => new
                {
                    levelRange = $"Level {g.Key}-{g.Key + 9}",
                    count = g.Count()
                })
                .ToList();

            return Json(distribution);
        }

        // 獲取寵物皮膚顏色分佈
        [HttpGet]
        public async Task<IActionResult> GetPetTypeDistribution()
        {
            var allPets = await _petService.GetAllPetsAsync();

            var distribution = allPets
                .GroupBy(p => p.SkinColor)
                .Select(g => new
                {
                    skinColor = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(g => g.count)
                .ToList();

            return Json(distribution);
        }

        // 獲取寵物健康度統計
        [HttpGet]
        public async Task<IActionResult> GetPetDropRateStats()
        {
            var allPets = await _petService.GetAllPetsAsync();

            var stats = new
            {
                avgHealth = allPets.Any() ? allPets.Average(p => (double)p.Health) : 0,
                avgHunger = allPets.Any() ? allPets.Average(p => (double)p.Hunger) : 0,
                avgMood = allPets.Any() ? allPets.Average(p => (double)p.Mood) : 0,
                avgStamina = allPets.Any() ? allPets.Average(p => (double)p.Stamina) : 0,
                avgCleanliness = allPets.Any() ? allPets.Average(p => (double)p.Cleanliness) : 0
            };

            return Json(stats);
        }

        // 獲取寵物顏色選項
        [HttpGet]
        public async Task<IActionResult> GetPetColorOptions()
        {
            try
            {
                var options = await _petService.GetAvailableColorsAsync();
                return Json(new { success = true, data = options });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 新增寵物顏色選項
        [HttpPost]
        public async Task<IActionResult> AddPetColorOption(string colorName, string colorValue, int displayOrder)
        {
            try
            {
                if (string.IsNullOrEmpty(colorName) || string.IsNullOrEmpty(colorValue))
                {
                    return Json(new { success = false, message = "顏色名稱和顏色值不能為空" });
                }

                var option = new PetColorOption
                {
                    ColorName = colorName,
                    ColorValue = colorValue,
                    DisplayOrder = displayOrder,
                    IsActive = true
                };

                var result = await _petRulesService.CreateColorOptionAsync(option);

                if (result)
                {
                    return Json(new { success = true, message = "寵物顏色選項新增成功" });
                }
                else
                {
                    return Json(new { success = false, message = "新增失敗" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 更新寵物顏色選項
        [HttpPost]
        public async Task<IActionResult> UpdatePetColorOption(int id, string colorName, string colorValue, int displayOrder, bool isActive)
        {
            try
            {
                var options = await _petRulesService.GetAllColorOptionsAsync();
                var option = options.FirstOrDefault(o => o.ColorOptionId == id);

                if (option == null)
                {
                    return Json(new { success = false, message = "找不到指定的顏色選項" });
                }

                option.ColorName = colorName;
                option.ColorValue = colorValue;
                option.DisplayOrder = displayOrder;
                option.IsActive = isActive;

                var result = await _petRulesService.UpdateColorOptionAsync(option);

                if (result)
                {
                    return Json(new { success = true, message = "寵物顏色選項更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "更新失敗" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 刪除寵物顏色選項
        [HttpPost]
        public async Task<IActionResult> DeletePetColorOption(int id)
        {
            try
            {
                var result = await _petRulesService.DeleteColorOptionAsync(id);

                if (result)
                {
                    return Json(new { success = true, message = "寵物顏色選項刪除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "刪除失敗" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 獲取寵物背景選項
        [HttpGet]
        public async Task<IActionResult> GetPetBackgroundOptions()
        {
            try
            {
                var options = await _petService.GetAvailableBackgroundsAsync();
                return Json(new { success = true, data = options });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 獲取升級規則
        [HttpGet]
        public async Task<IActionResult> GetPetLevelUpRules()
        {
            try
            {
                var rules = await _petRulesService.GetAllLevelUpRulesAsync();
                return Json(new { success = true, data = rules });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 更新升級規則
        [HttpPost]
        public async Task<IActionResult> UpdatePetLevelUpRules(List<PetLevelUpRule> rules)
        {
            try
            {
                if (rules == null || !rules.Any())
                {
                    return Json(new { success = false, message = "升級規則不能為空" });
                }

                // 更新每個規則
                foreach (var rule in rules)
                {
                    await _petRulesService.UpdateLevelUpRuleAsync(rule);
                }

                return Json(new { success = true, message = "寵物升級規則更新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 獲取互動獎勵規則
        [HttpGet]
        public async Task<IActionResult> GetPetInteractionBonusRules()
        {
            try
            {
                var rules = await _petRulesService.GetAllInteractionRulesAsync();
                return Json(new { success = true, data = rules });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 更新互動獎勵規則
        [HttpPost]
        public async Task<IActionResult> UpdatePetInteractionBonusRules(List<PetInteractionBonusRules> rules)
        {
            try
            {
                if (rules == null || !rules.Any())
                {
                    return Json(new { success = false, message = "互動狀態增益規則不能為空" });
                }

                // 更新每個規則
                foreach (var rule in rules)
                {
                    await _petRulesService.UpdateInteractionRuleAsync(rule);
                }

                return Json(new { success = true, message = "寵物互動狀態增益規則更新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 獲取換色成本
        [HttpGet]
        public async Task<IActionResult> GetPetColorChangeCost()
        {
            try
            {
                // 使用預設等級1的成本
                var cost = await _petRulesService.GetSkinColorCostForLevelAsync(1);
                return Json(new { success = true, data = new { pointsRequired = cost } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 更新換色成本
        [HttpPost]
        public async Task<IActionResult> UpdatePetColorChangeCost(int pointsRequired)
        {
            try
            {
                if (pointsRequired < 0)
                {
                    return Json(new { success = false, message = "所需點數不能為負數" });
                }

                // 更新等級1的換色成本設定
                var settings = await _petRulesService.GetAllSkinColorSettingsAsync();
                var setting = settings.FirstOrDefault(s => s.RequiredLevel == 1);

                if (setting != null)
                {
                    setting.PointCost = pointsRequired;
                    await _petRulesService.UpdateSkinColorSettingAsync(setting);
                }

                return Json(new { success = true, message = "寵物換色所需點數設定成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 獲取換背景成本
        [HttpGet]
        public async Task<IActionResult> GetPetBackgroundChangeCost()
        {
            try
            {
                var cost = await _petRulesService.GetBackgroundCostForLevelAsync(1);
                return Json(new { success = true, data = new { pointsRequired = cost } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 更新換背景成本
        [HttpPost]
        public async Task<IActionResult> UpdatePetBackgroundChangeCost(int pointsRequired)
        {
            try
            {
                if (pointsRequired < 0)
                {
                    return Json(new { success = false, message = "所需點數不能為負數" });
                }

                var settings = await _petRulesService.GetAllBackgroundSettingsAsync();
                var setting = settings.FirstOrDefault(s => s.RequiredLevel == 1);

                if (setting != null)
                {
                    setting.PointCost = pointsRequired;
                    await _petRulesService.UpdateBackgroundSettingAsync(setting);
                }

                return Json(new { success = true, message = "寵物換背景所需點數設定成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 系統規則設定頁面
        /// </summary>
        public async Task<IActionResult> SystemRules()
        {
            try
            {
                // 獲取寵物等級經驗設定
                var levelExperienceSettings = await _context.PetLevelExperienceSettings
                    .AsNoTracking()
                    .OrderBy(s => s.Level)
                    .ToListAsync();

                // 獲取寵物升級規則
                var levelUpRules = await _context.PetLevelUpRules
                    .AsNoTracking()
                    .ToListAsync();

                // 獲取寵物互動獎勵規則
                var interactionBonusRules = await _context.PetInteractionBonusRules
                    .AsNoTracking()
                    .ToListAsync();

                // 獲取寵物顏色選項
                var colorOptions = await _context.PetColorOptions
                    .AsNoTracking()
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                // 獲取寵物背景選項
                var backgroundOptions = await _context.PetBackgroundOptions
                    .AsNoTracking()
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.SortOrder)
                    .ToListAsync();

                // 獲取寵物換色所需點數設定
                var skinColorCostSettings = await _context.PetSkinColorCostSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                // 獲取寵物換背景所需點數設定
                var backgroundCostSettings = await _context.PetBackgroundCostSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                var model = new PetSystemRulesViewModel
                {
                    LevelExperienceSettings = levelExperienceSettings,
                    LevelUpRules = levelUpRules,
                    InteractionBonusRules = interactionBonusRules,
                    ColorOptions = colorOptions,
                    BackgroundOptions = backgroundOptions,
                    SkinColorCostSettings = skinColorCostSettings,
                    BackgroundCostSettings = backgroundCostSettings
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入系統規則設定時發生錯誤：{ex.Message}";
                return View(new PetSystemRulesViewModel());
            }
        }

        /// <summary>
        /// 會員個別寵物設定
        /// </summary>
        public async Task<IActionResult> IndividualSettings(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var pet = await _context.Pets
                    .AsNoTracking()
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.PetId == id);

                if (pet == null)
                {
                    return NotFound();
                }

                // 獲取可用的顏色選項
                var colorOptions = await _context.PetColorOptions
                    .AsNoTracking()
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                // 獲取可用的背景選項
                var backgroundOptions = await _context.PetBackgroundOptions
                    .AsNoTracking()
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.SortOrder)
                    .ToListAsync();

                var model = new PetIndividualSettingsViewModel
                {
                    PetId = pet.PetId,
                    UserId = pet.UserId,
                    UserName = pet.User.UserName,
                    PetName = pet.PetName,
                    CurrentSkinColor = pet.SkinColor,
                    CurrentBackground = pet.Background,
                    ColorOptions = colorOptions,
                    BackgroundOptions = backgroundOptions
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入寵物設定時發生錯誤：{ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 會員個別寵物清單與查詢
        /// </summary>
        public async Task<IActionResult> QueryPets(string searchTerm = "", string sortBy = "name", int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Pets
                    .AsNoTracking()
                    .Include(p => p.User)
                    .AsQueryable();

                // 搜尋功能
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    if (int.TryParse(searchTerm, out int userId))
                    {
                        query = query.Where(p => p.UserId == userId);
                    }
                    else
                    {
                        query = query.Where(p => p.PetName.Contains(searchTerm) || p.User.UserName.Contains(searchTerm));
                    }
                }

                // 排序
                query = sortBy switch
                {
                    "level" => query.OrderByDescending(p => p.Level),
                    "experience" => query.OrderByDescending(p => p.Experience),
                    "user" => query.OrderBy(p => p.User.UserName),
                    _ => query.OrderBy(p => p.PetName)
                };

                // 分頁
                var totalCount = await query.CountAsync();
                var pets = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PetQueryRecord
                    {
                        PetId = p.PetId,
                        UserId = p.UserId,
                        UserName = p.User.UserName,
                        PetName = p.PetName,
                        Level = p.Level,
                        Experience = p.Experience,
                        SkinColor = p.SkinColor,
                        Background = p.BackgroundColor,
                        Happiness = p.Mood,
                        Health = p.Health,
                        Hunger = p.Hunger,
                        Energy = p.Stamina,
                        Intelligence = p.Cleanliness
                    })
                    .ToListAsync();

                var model = new PetQueryViewModel
                {
                    SearchTerm = searchTerm,
                    SortBy = sortBy,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Pets = pets
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入寵物清單時發生錯誤：{ex.Message}";
                return View(new PetQueryViewModel());
            }
        }

        /// <summary>
        /// 換膚色/背景紀錄
        /// </summary>
        public async Task<IActionResult> ColorChangeHistory(int? userId = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 20, string changeType = "skin")
        {
            try
            {
                var query = _context.WalletHistories
                    .AsNoTracking()
                    .Include(h => h.User)
                    .Where(h => h.ChangeType == (changeType == "skin" ? "寵物換膚色" : "寵物換背景"))
                    .AsQueryable();

                // 使用者篩選
                if (userId.HasValue)
                {
                    query = query.Where(h => h.UserId == userId.Value);
                }

                // 日期篩選
                if (startDate.HasValue)
                {
                    query = query.Where(h => h.ChangeTime >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(h => h.ChangeTime <= endDate.Value);
                }

                // 排序
                query = query.OrderByDescending(h => h.ChangeTime);

                // 分頁
                var totalCount = await query.CountAsync();
                var records = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new ColorChangeRecord
                    {
                        LogId = h.LogId,
                        UserId = h.UserId,
                        UserName = h.User.UserName,
                        ChangeType = h.ChangeType,
                        ChangeTime = h.ChangeTime,
                        PointsChange = h.PointsChanged,
                        Description = h.Description
                    })
                    .ToListAsync();

                var model = new ColorChangeHistoryViewModel
                {
                    UserId = userId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ChangeType = changeType,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Records = records
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入換色紀錄時發生錯誤：{ex.Message}";
                return View(new ColorChangeHistoryViewModel());
            }
        }
    }
}

