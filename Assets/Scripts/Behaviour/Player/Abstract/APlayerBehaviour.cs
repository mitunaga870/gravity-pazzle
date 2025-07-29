#region

using Behaviour.Camera;
using UnityEngine;

#endregion

namespace Behaviour.Player.Abstract
{
    /// <summary>
    /// プレイヤーの挙動を持つオブジェクトの抽象クラス
    /// 実装内容：WASD移動
    /// </summary>
    public abstract class APlayerBehaviour : MonoBehaviour
    {
        #region SerializeField
        [SerializeField]
        private Rigidbody playerRigidBody;
        
        [SerializeField]
        private Animator playerAnimator;
        
        [SerializeField]
        protected PlayerCam playerCam;

        #endregion

        #region Public Properties

        // チュートリアル用の状態プロパティ
        // プレイヤーが移動したかどうか
        public bool IsMoved { get; private set; }

        #endregion

        #region Unity Methods
        protected void Update()
        {
            // プレイヤーの移動
            var moveDirection = GetMoveDirection(Time.deltaTime);
            playerRigidBody.MovePosition(playerRigidBody.position + moveDirection);

            // 移動したかどうかを更新
            IsMoved = IsMoved || moveDirection != Vector3.zero;
            
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

        #endregion

        /**
         * 移動ベクトルを取得する
         */
        protected abstract Vector3 GetMoveDirection(float deltaTime);
    }
}