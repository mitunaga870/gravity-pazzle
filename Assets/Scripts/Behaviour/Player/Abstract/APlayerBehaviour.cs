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

        protected void Update()
        {
            // プレイヤーの移動
            var moveDirection = GetMoveDirection(Time.deltaTime);
            playerRigidBody.MovePosition(playerRigidBody.position + moveDirection);
        }

        /**
         * 移動ベクトルを取得する
         */
        protected abstract Vector3 GetMoveDirection(float deltaTime);
    }
}