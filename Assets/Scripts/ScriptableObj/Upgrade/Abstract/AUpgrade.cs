#region

using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    /// <summary>
    ///     アップグレード情報のデータ抽象クラス
    ///     これのまま使わずに、レベルあるいは飲んレベルの物を使って創こと
    /// </summary>
    public abstract class AUpgrade : ScriptableObject
    {
        [SerializeField]
        private string displayName;

        public string DisplayName => displayName;

        [SerializeField]
        private UpgradeableCondition[] upgradeableConditions;

        public UpgradeableCondition[] UpgradeableConditions => upgradeableConditions;

        public abstract UpgradeCategory UpgradeCategory { get; }
    }

    public enum UpgradeCategory
    {
        Pram,
        Action
    }
}