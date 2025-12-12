#region

using System;
using System.Linq;
using Lib.DataClass.PlayData;
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
        private UpgradeType upgradeType;

        public UpgradeType UpgradeType => upgradeType;

        [SerializeField]
        private int[] cost;

        public int[] Cost => cost;

        [SerializeField]
        private UpgradeableCondition[] upgradeableConditions;

        public UpgradeableCondition[] UpgradeableConditions => upgradeableConditions;

        public abstract UpgradeCategory UpgradeCategory { get; }

        /// <summary>
        ///     設定したアップグレード可能バージョンに対応しているか確認する
        ///     対応レベルかどうかの判断はそれぞれのカテゴリでやること
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
        protected bool IsUpgradeableCondition(PlayerData playerData)
        {
            var curLevel = playerData.GetLevel(upgradeType);
            var nextLevel = curLevel + 1;

            // コンディションがない場合は問答無用強化
            if (UpgradeableConditions.Length == 0)
                return true;
            
            // nextLevel用のコンディションを取得
            var applyCondition =
                UpgradeableConditions.First(condition => condition.TargetLevel == nextLevel).ConditionLevels;

            // 実際のチェック
            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                var otherLevel = playerData.GetLevel(type);
                var borderLevel = applyCondition[type];

                if (otherLevel < borderLevel) return false;
            }

            return true;
        }

        /// <summary>
        ///     アップグレード可能か判断する
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
        public abstract bool IsUpgradeable(PlayerData playerData);
    }

    public enum UpgradeCategory
    {
        Pram,
        Action
    }
}