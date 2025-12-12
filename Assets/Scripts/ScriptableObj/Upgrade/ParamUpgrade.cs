#region

using Lib.DataClass.PlayData;
using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    /// <summary>
    ///     パラメータで制御するアップグレード
    /// </summary>
    [CreateAssetMenu(fileName = "強化情報", menuName = "ScriptableObj/強化情報/パラメータ強化")]
    public class ParamUpgrade : AUpgrade
    {
        public override UpgradeCategory UpgradeCategory => UpgradeCategory.Pram;

        [Header("レベルごとのパラメーター（強化前は省略）")]
        [SerializeField]
        private float[] upgradedParams;

        public float[] UpgradedParams => upgradedParams;

        protected override bool IsUpgradeableForCategory(PlayerData playerData)
        {
            var nextLevel = playerData.GetLevel(UpgradeType) + 1;

            return nextLevel >= upgradedParams.Length;
        }
    }
}