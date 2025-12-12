#region

using Lib.DataClass.PlayData;
using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    [CreateAssetMenu(fileName = "強化情報", menuName = "ScriptableObj/強化情報/アクション強化")]
    public class ActionUpgrade : AUpgrade
    {
        public override UpgradeCategory UpgradeCategory => UpgradeCategory.Action;

        public override bool IsUpgradeable(PlayerData playerData)
        {
            var curLevel = playerData.GetLevel(UpgradeType);

            // アクションは0か１しかないので0の時だけ可能
            var allowLevel = curLevel == 0;

            // コンディションも確認
            var allowCondition = IsUpgradeableCondition(playerData);

            return allowCondition && allowLevel;
        }
    }
}