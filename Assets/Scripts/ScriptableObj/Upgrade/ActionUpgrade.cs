#region

using Lib.DataClass.PlayData;
using ScriptableObj.Upgrade.Abstract;
using UnityEngine;

#endregion

namespace ScriptableObj.Upgrade
{
    [CreateAssetMenu(fileName = "強化情報", menuName = "ScriptableObj/強化情報/アクション強化")]
    public class ActionUpgrade : AUpgrade
    {
        public override UpgradeCategory UpgradeCategory => UpgradeCategory.Action;

        [SerializeField]
        private string content;
        
        public string Content => content;

        protected override bool IsUpgradeableForCategory(PlayerData playerData)
        {
            var curLevel = playerData.GetLevel(UpgradeType);

            // アクションは0か１しかないので0の時だけ可能
            var allowLevel = curLevel == 0;

            return allowLevel;
        }
    }
}