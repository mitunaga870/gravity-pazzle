#region

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


        /// <summary>
        ///     次のレベルのパラメータを取得する
        ///     強化不能の場合はfalseを返す
        /// </summary>
        /// <param name="level">要望レベル</param>
        /// <param name="nextParam">次のパラメータ</param>
        /// <returns></returns>
        public bool GetNextParam(int level, out float nextParam)
        {
            if (level < upgradedParams.Length)
            {
                nextParam = upgradedParams[level];
                return false;
            }

            nextParam = upgradedParams[level];
            return true;
        }
    }
}