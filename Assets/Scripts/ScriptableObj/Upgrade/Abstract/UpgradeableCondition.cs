#region

using System;
using Lib.DataClass.ForInspector;
using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    /// <summary>
    ///     アップグレード可能になる場合の条件を設定するクラス
    /// </summary>
    [Serializable]
    public class UpgradeableCondition
    {
        [Header("条件を適用するレベル")]
        [SerializeField]
        private int targetLevel;

        [Header("アップグレード条件　アップグレードごとにレベルを登録")]
        [SerializeField]
        private UpgradeableConditionValue conditionLevels;
    }

    /// <summary>
    ///     アップグレードの種類
    /// </summary>
    public enum UpgradeType
    {
        OperationDuration = 0,
        MaxOperationCount = 1,
        PlayerGravChange = 2
    }

    [Serializable]
    public class UpgradeableConditionValue : AEnumArray<UpgradeType, int>
    {
    }
}