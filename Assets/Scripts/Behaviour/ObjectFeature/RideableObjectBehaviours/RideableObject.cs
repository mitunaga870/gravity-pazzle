using System;
using Behaviour.ObjectFeature.RideableObjectBehaviours;
using Behaviour.Trigger;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviour.ObjectFeature
{
    /// <summary>
    /// 乗れるオブジェクト用クラス
    /// Riderタグのついたオブジェクトが触った時に重力底面のみ子供にする
    /// </summary>
    public class RideableObject : MonoBehaviour
    {
        [SerializeField]
        private RideTrigger[] triggers;

        private void Start()
        {
            // トリガーに親子操作を追加
            foreach (var rideTrigger in triggers)
            {
                rideTrigger.OnRiderEnter += (rider =>
                {
                    rider.RidingObject = gameObject;
                });

                rideTrigger.OnRiderExit += (rider =>
                {
                    // 乗ってるオブジェクトが自分の時のみ実行
                    if (rider.RidingObject != gameObject)
                        return;
                    
                    rider.RidingObject = null;
                });
            }
        }
    }
}