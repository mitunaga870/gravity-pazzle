#region

using System;
using ScriptableObj.Upgrade;

#endregion

namespace Lib.Logic
{
    public static class UpgradeUtils
    {
        public static UpgradeCategory GetCategory(UpgradeType upgradeType)
        {
            return upgradeType switch
            {
                UpgradeType.OperationDuration or UpgradeType.MaxOperationCount => UpgradeCategory.Param,
                UpgradeType.PlayerGravChange => UpgradeCategory.Action,
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }
    }
}