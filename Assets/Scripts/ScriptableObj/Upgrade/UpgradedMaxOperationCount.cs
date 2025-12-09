#region

using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    /// <summary>
    ///     強化後の重力制限個数を設定
    /// </summary>
    [CreateAssetMenu(fileName = "強化後最大個数", menuName = "ScriptableObj/強化後パラメータ")]
    public class UpgradedMaxOperationCount : ScriptableObject
    {
        [SerializeField]
        private int[] upgradedOperationCount;
    }
}