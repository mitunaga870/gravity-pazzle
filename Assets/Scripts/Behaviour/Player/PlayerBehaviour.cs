#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Gravity;
using Behaviour.Player.Abstract;
using Behaviour.UI;
using Lib.DataClass.Settings.GravSelectMethod;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using Lib.State.Scene;
using ScriptableObj.Setting;
using UnityEngine;

#endregion

namespace Behaviour.Player
{
    /// <summary>
    ///     プレイヤー用の挙動クラス
    ///     プレイヤー移動とカメラへのプレイヤー位置の通知を行う
    /// </summary>
    [RequireComponent(typeof(PlayerAnimBehaviour))]
    public class PlayerBehaviour : APlayerBehaviour
    {
        private const float Speed = 5f;

        private PlayerKey PlayerKey => SettingDataController.Instance.PlayerKey;

        [Header("プレイヤー設定")]
        [SerializeField]
        private bool changeableGrav = true;

        [Header("参照用")]

        [SerializeField]
        private DirectionUIWrapper directionUIWrapper;


        private GravType _targetGravType = GravType.XNegative;
        private PlayerAnimBehaviour _animBehaviour;

        #region Unity Methods

        private new void Start()
        {
            base.Start();

            if (GravBehaviour == null)
                Debug.LogError("GravBehaviour is not assigned.");
            _animBehaviour = GetComponent<PlayerAnimBehaviour>();
        }

        private new void Update()
        {
            // 既定を継承しているので、Updateメソッドをオーバーライド
            base.Update();

            // スペースで影響を受けているならフローティングに変換
            SetGrav();
            UnsetGrav();

            // 右クリックでターゲットの方向を変更
            SetGravDirection();

            // スペースキーでプレイヤーの重力を設定済み方向に変更
            if (input.GetKeyDown(PlayerKey.SetPlayerGravKey) && changeableGrav)
            {
                var playerVGrav = GravBehaviour as VGravBehaviour;
                if (playerVGrav != null) playerVGrav.SetGravAffected(_targetGravType);
            }

            // 元に戻す
            if (input.GetKeyDown(PlayerKey.UnsetPlayerGravKey) && changeableGrav)
            {
                var playerVGrav = GravBehaviour as VGravBehaviour;
                if (playerVGrav != null) playerVGrav.UnsetGravAffected();
            }

            // 落下方向速度を取得
            var velocity = PlayerRigidBody.linearVelocity;
            var gravDirection = GravUtils.GetGravDirectionUnit(GravBehaviour.GravType);
            var fallVelocity = Vector3.Dot(velocity, gravDirection);
            const float fallThreshold = 0.1f;
            // 重力変更中かどうか
            var isGravChanging = GravBehaviour.IsGravAdapting;
            // 接地しているか
            var isGrounded = Physics.Raycast(transform.position, gravDirection, gravDirection.magnitude);
            // 落下しているか
            var isFalling =
                (fallVelocity > fallThreshold && !isGrounded) ||
                (!isGrounded && isGravChanging);

            // 落下をアニメーションに通知
            _animBehaviour.IsFalling(isFalling);

            // カメラに位置を通知
            PlayerCam.SetPlayerPosAndGrav(transform, GravBehaviour.GravType);
        }

        #endregion

        #region APlayerBehaviour Methods

        protected override Vector3 GetMoveSpeed()
        {
            // WASDキーの入力を取得
            var xInput = input.GetKey(PlayerKey.MoveForwardKey, SceneState.InGame);
            var zInput = input.GetKey(PlayerKey.MoveBackwardKey, SceneState.InGame);
            var yInput = input.GetKey(PlayerKey.MoveLeftKey, SceneState.InGame);
            var wInput = input.GetKey(PlayerKey.MoveRightKey, SceneState.InGame);

            // 負荷軽減のため、入力がない場合は移動しない
            if (!xInput && !zInput && !yInput && !wInput)
            {
                _animBehaviour.UpdateSpeed(0);
                return Vector3.zero;
            }

            // 入力方向を計算
            var camTransform = PlayerCam.transform;
            var moveDirection = GravUtils.AdjustDirectionToGrav(
                xInput,
                yInput,
                zInput,
                wInput,
                camTransform.forward,
                camTransform.right,
                GravBehaviour.GravType
            );

            // 移動速度を掛けて、時間を掛けlる
            var moveSpeed = moveDirection * Speed;

            // アニメーターに移動量を0~1に正規化して出力
            var speed = Math.Clamp(moveDirection.magnitude, 0f, 1f);
            _animBehaviour.UpdateSpeed(speed);

            return moveSpeed;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     カメラの先のオブジェクトに重力影響を与える
        /// </summary>
        private void SetGrav()
        {
            if (!input.GetMouseButton((int)PlayerKey.SetObjGravButton, SceneState.InGame)) return;
            
            // カメラの先のオブジェクトを取得
            var target = PlayerCam.GetCameraTarget();
            if (target == null)
                return;

            // クリックしたオブジェクトの可変重力コンポーネントを取得
            var targetGravBehaviour = target.GetComponent<VGravBehaviour>();
            if (targetGravBehaviour == null)
                return;

            // ターゲット重力方向にセット
            targetGravBehaviour.SetGravAffected(_targetGravType);
        }

        /// <summary>
        ///     カメラの先のオブジェクトの重力影響を解除する
        /// </summary>
        private void UnsetGrav()
        {
            if (!input.GetMouseButton((int)PlayerKey.UnsetObjGravButton, SceneState.InGame))
                return;

            // カメラの先のオブジェクトを取得
            var target = PlayerCam.GetCameraTarget();
            if (target == null)
                return;

            // クリックしたオブジェクトの可変重力コンポーネントを取得
            var targetGravBehaviour = target.GetComponent<VGravBehaviour>();
            if (targetGravBehaviour == null)
                return;

            // ターゲット重力方向にセット
            targetGravBehaviour.UnsetGravAffected();
        }

        private void SetGravDirection()
        {
            var method = SettingDataController.Instance.UserSettings.GravSelectMethod;
            if (method == null) return;

            switch (method)
            {
                case Mouse:
                    SetDirectionByMouse();
                    break;
                case Keyboard:
                    SetDirectionByKeyboard();
                    break;
                default:
                    throw new Exception(
                        $"grav select method \"{method.DisplayName}\" is not implemented yet.");
            }

            // UIに重力方向を通知
            directionUIWrapper.SetGravType(_targetGravType);

            return;

            void SetDirectionByMouse()
            {
                if (!input.GetMouseButton((int)PlayerKey.ChangeGravDirectionMouseButton, SceneState.InGame))
                    return;

                // カメラの向いている方向を取得
                var camTransform = PlayerCam.transform;
                var camForward = camTransform.forward;

                // ターゲットの重力方向を変更
                _targetGravType = GravUtils.GetMaxDirection(camForward);

                // ターゲット重力変更済み
                IsTargetGravChanged = true;
            }

            void SetDirectionByKeyboard()
            {
                var modifier = PlayerKey.ChangeGravDirectionModifierKey;
                if (!input.GetKey(modifier, SceneState.InGame)) return;

                // wasd/space/ctrlでターゲットの重力方向を変更
                // wasdなら移動方向で最も大きい軸を重力方向に設定
                if (
                    input.GetKeyDown(PlayerKey.MoveForwardKey, SceneState.InGame) ||
                    input.GetKeyDown(PlayerKey.MoveBackwardKey, SceneState.InGame) ||
                    input.GetKeyDown(PlayerKey.MoveLeftKey, SceneState.InGame) ||
                    input.GetKeyDown(PlayerKey.MoveRightKey, SceneState.InGame)
                )
                {
                    // 移動方向から最大軸を取得
                    var moveDirection = GetMoveSpeed();
                    _targetGravType = GravUtils.GetMaxDirection(moveDirection);
                }
                else if (input.GetKeyDown(PlayerKey.ChangeGravDirectionToTopKey, SceneState.InGame))
                {
                    _targetGravType = GravUtils.GetUpperGravType(GravBehaviour.GravType);
                }
                else if (input.GetKeyDown(PlayerKey.ChangeGravDirectionToBottomKey, SceneState.InGame))
                {
                    _targetGravType = GravBehaviour.GravType;
                }
            }
        }

        #endregion
    }
}