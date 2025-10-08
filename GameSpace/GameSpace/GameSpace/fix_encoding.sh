#!/bin/bash

# Bash script to fix UTF-8 BOM encoding for all source files
# This script adds UTF-8 BOM to files that don't have it

files=(
    "Areas/MiniGame/Services/IPetColorOptionService.cs"
    "Areas/MiniGame/Services/IPetSkinColorCostSettingService.cs"
    "Areas/MiniGame/Services/IDailyGameLimitValidationService.cs"
    "Areas/MiniGame/Services/PetColorOptionService.cs"
    "Areas/MiniGame/Services/IPetBackgroundOptionService.cs"
    "Areas/MiniGame/Services/IPetBackgroundCostSettingService.cs"
    "Areas/MiniGame/Services/PetSkinColorCostSettingService.cs"
    "Areas/MiniGame/Services/PetBackgroundCostSettingService.cs"
    "Areas/MiniGame/Views/PetLevelUpRuleValidation/Index.cshtml"
    "Areas/MiniGame/Views/PetBackgroundCostSetting/Create.cshtml"
    "Areas/MiniGame/Views/AdminPet/IndividualSettings.cshtml"
    "Areas/MiniGame/Views/AdminPet/ListWithQuery.cshtml"
    "Areas/MiniGame/Views/AdminPet/ColorChangeHistory.cshtml"
    "Areas/MiniGame/Views/PetSkinColorCostSetting/Create.cshtml"
    "Areas/MiniGame/Views/Permission/OperationLogs.cshtml"
    "Areas/MiniGame/Views/AdminMiniGame/Create.cshtml"
    "Areas/MiniGame/Views/AdminSignIn/Rules.cshtml"
    "Areas/MiniGame/Views/AdminWallet/GrantCoupons.cshtml"
    "Areas/MiniGame/Views/AdminWallet/AdjustPoints.cshtml"
    "Areas/MiniGame/Views/AdminWallet/QueryHistory.cshtml"
    "Areas/MiniGame/Views/AdminWallet/Transaction.cshtml"
    "Areas/MiniGame/Views/AdminWallet/AdjustEVouchers.cshtml"
    "Areas/MiniGame/Views/AdminWallet/ViewHistory.cshtml"
    "Areas/MiniGame/Views/AdminWallet/GrantEVouchers.cshtml"
    "Areas/MiniGame/Views/AdminManager/CreateRole.cshtml"
    "Areas/MiniGame/Views/AdminEVoucher/CreateType.cshtml"
    "Areas/MiniGame/Views/AdminEVoucher/Create.cshtml"
    "Areas/MiniGame/Views/EVouchers/Edit.cshtml"
    "Areas/MiniGame/Views/EVouchers/Create.cshtml"
    "Areas/MiniGame/Views/Admin/Index.cshtml"
    "Areas/MiniGame/Views/AdminCoupon/Create.cshtml"
    "Areas/MiniGame/Views/AdminUser/Edit.cshtml"
    "Areas/MiniGame/Views/AdminUser/Create.cshtml"
    "Areas/MiniGame/ViewModels/SignInRuleConfigViewModel.cs"
    "Areas/MiniGame/Models/Settings/PetBackgroundPointSettings.cs"
    "Areas/MiniGame/Models/Settings/PetSkinColorPointSettings.cs"
    "Areas/MiniGame/Models/ManagerData.cs"
    "Areas/MiniGame/Models/ViewModels/PetColorChangeSettingsViewModel.cs"
    "Areas/MiniGame/Models/ViewModels/PermissionLogQueryModel.cs"
    "Areas/MiniGame/Models/ViewModels/UserRightUpdateModel.cs"
    "Areas/MiniGame/Models/ViewModels/PetColorOptionViewModel.cs"
    "Areas/MiniGame/Models/ViewModels/UserRightModel.cs"
    "Areas/MiniGame/Models/ViewModels/CouponTypeViewModel.cs"
    "Areas/MiniGame/Models/ViewModels/UserManagementViewModels.cs"
    "Areas/MiniGame/Models/ViewModels/PagedResult.cs"
    "Areas/MiniGame/Models/ErrorLog.cs"
)

for file in "${files[@]}"; do
    if [ -f "$file" ]; then
        echo "Fixing encoding for: $file"
        # Check if file already has BOM
        if ! hexdump -C "$file" | head -1 | grep -q "ef bb bf"; then
            # Add UTF-8 BOM to the beginning of the file
            printf '\xef\xbb\xbf' > temp_file
            cat "$file" >> temp_file
            mv temp_file "$file"
        fi
    else
        echo "File not found: $file"
    fi
done

echo "Encoding fix completed!"