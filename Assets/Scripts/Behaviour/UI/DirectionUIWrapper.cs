using Behaviour.ObjectFeature;
using Lib.State.Interface.Gravity;
using UnityEngine;

namespace Behaviour.UI
{
    public class DirectionUIWrapper : MonoBehaviour
    {
        [SerializeField]
        private VisibleWithGravType[] visibleWithGravType;
        
        public void SetGravType(GravType gravType)
        {
            foreach (var item in visibleWithGravType)
            {
                item.SettingGravType = gravType;
            }
        }
    }
}