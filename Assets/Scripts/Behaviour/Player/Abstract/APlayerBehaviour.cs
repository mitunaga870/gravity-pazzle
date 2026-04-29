#region

using System;
using Behaviour.Camera;
using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
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

        protected Rigidbody PlayerRigidBody => playerRigidBody;

        #endregion

        #region Private Fields

        private const float AccelerationTime = 0.2f;
        private const string FootstepsSeId = "Footsteps";
        private const string FallSeId = "Fall";
        private const float FallSeFadeOutSeconds = 0.2f;

        private bool _wasMoved;
        private bool _isFalling;
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
            {
                HandleFootstepsSe(false);
                return;
            }
            
            // プレイヤーの移動
            var moveSpeed = GetMoveSpeed();
            playerRigidBody.MovePosition(transform.position + moveSpeed * Time.deltaTime);
            var isMoving = moveSpeed != Vector3.zero;

            // 移動したかどうかを更新
            IsFirstMoved = IsFirstMoved || isMoving;

            HandleFootstepsSe(isMoving);

            // 速度があれば、移動方向を向く
            if (isMoving)
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

        public void SetFallingState(bool isFalling)
        {
            if (_isFalling == isFalling) return;
            _isFalling = isFalling;

            var soundController = SoundController.Instance;
            if (soundController == null) return;

            if (_isFalling)
            {
                if (soundController.IsLoopSePlaying)
                    soundController.StopLoopSe();
                soundController.PlayLoopSe(FallSeId);
                _wasMoved = false;
                return;
            }

            if (soundController.IsLoopSePlaying)
                soundController.StopLoopSeWithFade(FallSeFadeOutSeconds);
        }

        private void HandleFootstepsSe(bool isMoving)
        {
            if (_isFalling)
            {
                _wasMoved = false;
                return;
            }

            if (_wasMoved == isMoving) return;

            var soundController = SoundController.Instance;
            if (soundController == null)
            {
                _wasMoved = isMoving;
                return;
            }

            // Fall SE のフェードアウト中は Footsteps の再生開始を遅らせる
            if (isMoving && soundController.IsLoopSeFadingOut)
            {
                _wasMoved = false;
                return;
            }

            if (isMoving)
                soundController.PlayLoopSe(FootstepsSeId);
            else if (soundController.IsLoopSePlaying)
                soundController.StopLoopSe();

            _wasMoved = isMoving;
        }
    }
}