#region

using Behaviour.Gravity.Abstract;
using Behaviour.Trigger;
using Lib.Logic.Gravity;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Behaviour.ObjectFeature.RideableObjectBehaviours
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
                    // 自身に既に乗ってる時は処理しない
                    if (rider.RidingObject == gameObject)
                        return;

                    // 自身を搭乗オブジェクトに設定
                    rider.RidingObject = gameObject;

                    Debug.Log($"Rider {rider.name} ride on {gameObject.name}");
                });

                rideTrigger.OnRiderExit += (rider =>
                {
                    // 乗ってるオブジェクトが自分の時のみ実行
                    if (rider.RidingObject != gameObject)
                        return;

                    // 自身から降りた時は乗ってるオブジェクトをnullにする
                    rider.RidingObject = null;

                    Debug.Log($"Rider {rider.name} ride off {gameObject.name}");
                });
            }
        }

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
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                // 重力の影響を受ける場合は重力方向のみの速度を残す
                var velocity = rb.linearVelocity;
                var gravType = gravBehaviour.GravType;
                var gravDirection = GravUtils.GetGravDirectionUnit(gravType);
                
                var target =
                    new Vector3(
                        velocity.x * Mathf.Abs(gravDirection.x),
                        velocity.y * Mathf.Abs(gravDirection.y),
                        velocity.z * Mathf.Abs(gravDirection.z)
                        );

                // 速度を設定
                rb.linearVelocity = target;
            }
        }
    }
}