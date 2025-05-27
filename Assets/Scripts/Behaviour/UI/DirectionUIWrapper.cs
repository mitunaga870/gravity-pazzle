#region

using Behaviour.ObjectFeature;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.UI
{
    public class DirectionUIWrapper : MonoBehaviour
    {
        [SerializeField]
        private VisibleWithGravType[] visibleWithGravType;

        /// <summary>
        ///     設定された重力タイプに基づいてUIの可視性を更新します。
        /// </summary>
        /// <param name="gravType"></param>
        public void SetGravType(GravType gravType)
        {
            foreach (var item in visibleWithGravType)
            {
                item.SettingGravType = gravType;
            }
        }
    }
}