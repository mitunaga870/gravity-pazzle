using System;
using Behaviour.Gravity.Abstract;
using Behaviour.ObjectFeature.RideableObjectBehaviours;
using Behaviour.Trigger;
using Lib.Logic.Gravity;
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
        
        [FormerlySerializedAs("gravitySurface")]
        [SerializeField]
        private AGravBehaviour gravBehaviour;

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

        [Obsolete("Obsolete")]
        private void OnCollisionEnter(Collision other)
        {
            // ライダーコンポーネントがついているオブジェクトに乗った時
            var rider = other.gameObject.GetComponent<RiderObject>();
            if (rider == null) return;
            
            // 何かに乗ってない時は処理しない
            if (!rider.IsRiding) return;
            
            // リジットボディを取得
            var rb = other.gameObject.GetComponent<Rigidbody>();
            if (rb == null) return;
            
        
            // 乗ったオブジェクトの速度を消す
            if (gravBehaviour == null)
            {
                // 重力の影響を受けない場合
                rb.velocity = Vector3.zero;
            }
            else
            {
                var velocity = rb.velocity;
                var gravType = gravBehaviour.GravType;
                var gravDirection = GravUtils.GetGravDirectionUnit(gravType);
                
                var target =
                    new Vector3(
                        velocity.x * Mathf.Abs(gravDirection.x),
                        velocity.y * Mathf.Abs(gravDirection.y),
                        velocity.z * Mathf.Abs(gravDirection.z)
                        );
                
                rb.velocity = target;
            }
        }
    }
}