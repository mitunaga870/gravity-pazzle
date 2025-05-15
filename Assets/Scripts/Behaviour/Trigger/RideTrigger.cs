using System;
using Behaviour.Gravity.Abstract;
using Behaviour.ObjectFeature.RideableObjectBehaviours;
using Lib.State.Interface.Gravity;
using UnityEngine;

namespace Behaviour.Trigger
{
    /// <summary>
    /// 乗った時に関数を呼び出す
    /// </summary>
    public class RideTrigger : MonoBehaviour
    {
        [SerializeField]
        private GravType RiderGravType;
        
        public Action<RiderObject> OnRiderEnter;
        public Action<RiderObject> OnRiderExit;
        
        private void OnTriggerEnter(Collider other)
        {
            // ライダーオブジェクトを取得
            var rider = other.GetComponent<RiderObject>();
            // Riderタグがないなら虫
            if (rider == null)
                return;
            
            // オブジェクトのGravBehaviourを取得
            var gravBehaviour = other.GetComponent<AGravBehaviour>();
            if (gravBehaviour == null)
            {
                Debug.LogError("Rider does not have a GravBehaviour component.");
                return;
            }
            
            // 重力が一致していなければ虫
            if (gravBehaviour.GravType != RiderGravType)
            {
                return;
            }
            
            // アクション実施
            OnRiderEnter?.Invoke(rider);
        }

        private void OnTriggerExit(Collider other)
        {
            // RiderObjectを取得
            var rider = other.GetComponent<RiderObject>();
            // Riderタグがないなら虫
            if (rider == null)
                return;
            
            // アクション実施
            OnRiderExit?.Invoke(rider);
        }
    }
}