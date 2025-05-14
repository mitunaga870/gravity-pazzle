using Behaviour.Gravity.Abstract;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviour.ObjectFeature
{
    /// <summary>
    /// 設定された重力方向に基づいてオブジェクトの可視性を制御するクラス
    /// </summary>
    public class VisibleWithGravType :MonoBehaviour
    {
        [SerializeField]
        private GravType targetGravType;
        
        public GravType SettingGravType
        {
            set{
                _settingGravType = value;
                Update();
            }
        }
        private GravType _settingGravType;
        
        private void Update()
        {
            // 指定された重力タイプに基づいてオブジェクトの可視性を制御
            gameObject.SetActive(targetGravType == _settingGravType);
        }
    }
}