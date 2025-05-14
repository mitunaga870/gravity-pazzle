using Behaviour.Camera;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviour.Player.Abstract
{
    /// <summary>
    /// プレイヤーの挙動を持つオブジェクトの抽象クラス
    /// 実装内容：WASD移動
    /// </summary>
    public abstract class APlayerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody playerRigidBody;
        
        [SerializeField]
        private Animator playerAnimator;
        
        [SerializeField]
        protected PlayerCam playerCam;

        protected void Update()
        {
            // プレイヤーの移動
            var moveDirection = GetMoveDirection(Time.deltaTime);
            playerRigidBody.MovePosition(playerRigidBody.position + moveDirection);
            
            // 速度があれば、移動方向を向く
            if (moveDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(moveDirection);
                playerRigidBody.MoveRotation(Quaternion.Slerp(playerRigidBody.rotation, targetRotation, Time.deltaTime * 10f));
                
                // アニメーションの更新
                playerAnimator.SetBool("Walk", true);
                playerAnimator.SetBool("Idle", false);
            }
            else
            {
                // アニメーションの更新
                playerAnimator.SetBool("Walk", false);
                playerAnimator.SetBool("Idle", true);
            }
        }

        /**
         * 移動ベクトルを取得する
         */
        protected abstract Vector3 GetMoveDirection(float deltaTime);
    }
}