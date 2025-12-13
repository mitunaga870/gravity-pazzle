#region

using System;
using Behaviour.Camera;
using Behaviour.Controller.General;
using Behaviour.Controller.Stage;
using Behaviour.Gravity.Abstract;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Behaviour.Player.Abstract
{
    /// <summary>
    /// プレイヤーの挙動を持つオブジェクトの抽象クラス
    /// 実装内容：WASD移動
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AGravBehaviour))]
    public abstract class APlayerBehaviour : MonoBehaviour 
    {
        #region SerializeField
        [SerializeField]
        private Rigidbody playerRigidBody;
        
        
        [SerializeField]
        [Obsolete("カメラはStageDataControllerから取得するようになりました。")]
        protected PlayerCam playerCam;

        [FormerlySerializedAs("Input")]
        [SerializeField]
        protected InputController input;

        #endregion

        #region Public Properties

        // チュートリアル用の状態プロパティ
        // プレイヤーが移動したかどうか
        public bool IsFirstMoved { get; private set; }

        // チュートリアル用の状態フィールド
        // ターゲットの重力方向を変更したか
        public bool IsTargetGravChanged { get; protected set; }

        #endregion

        #region Protected Fields

        protected AGravBehaviour GravBehaviour { get; private set; }

        protected bool HasCam { get; private set; }
        protected PlayerCam PlayerCam { get; private set; }

        #endregion

        #region Private Fields

        private const float AccelerationTime = 0.2f;

        private bool _wasMoved;
        private float _acceleratedTime;

        #endregion

        #region Unity Methods

        protected void Start()
        {
            // SerializeFieldで設定されているかを確認
            if (playerRigidBody == null)
                Debug.LogError("Player Rigidbody is not assigned.");
            if (input == null)
                Debug.LogError("InputController is not assigned.");

            // 重力挙動コンポーネントを取得
            GravBehaviour = GetComponent<AGravBehaviour>();
            if (GravBehaviour == null)
                Debug.LogError("GravBehaviour component is not attached to the player.");

            // カメラを取得
            PlayerCam = StageDataController.Instance.PlayerCam;
            HasCam = PlayerCam != null;

            // ステージ設定にインスタンスIDを登録
            StageDataController.Instance.PlayerRigidbody = playerRigidBody;
        }
        
        protected void Update()
        {
            // 重力適応中は移動しない
            if (GravBehaviour.IsGravAdapting)
                return;
            
            // プレイヤーの移動
            var moveSpeed = GetMoveSpeed();
            playerRigidBody.MovePosition(transform.position + moveSpeed * Time.deltaTime);

            // 移動したかどうかを更新
            IsFirstMoved = IsFirstMoved || moveSpeed != Vector3.zero;

            // 速度があれば、移動方向を向く
            if (moveSpeed != Vector3.zero)
            {
                // 値が小さいとLookRotationでエラーになるため、適当な数をかける
                var adjustedMoveDirection = moveSpeed * 1000f;
                var upDirection = playerRigidBody.transform.up;
                var targetRotation = Quaternion.LookRotation(adjustedMoveDirection, upDirection);

                // Sharpを使わず、一発で回転させる
                playerRigidBody.transform.rotation = targetRotation;
            }
        }

        #endregion

        /**
         * 移動ベクトルを取得する
         */
        protected abstract Vector3 GetMoveSpeed();
    }
}