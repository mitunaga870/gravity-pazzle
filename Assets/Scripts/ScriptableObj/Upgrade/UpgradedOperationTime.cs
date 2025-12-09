#region

using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    /// <summary>
    ///     重力制限時間の各レベルごとの強化後秒数を設定
    /// </summary>
    public class UpgradedOperationTime : ScriptableObject
    {
        [SerializeField]
        private float[] upgradedOperationTime;
    }
}