#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Gravity;
using Behaviour.Gravity.Abstract;
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
    public class PlayerBehaviour : APlayerBehaviour
    {
        private const float Speed = 5f;

        [Header("操作設定")]
        [SerializeField]
        private PlayerKey playerKey;

        [Header("プレイヤー設定")]
        [SerializeField]
        private bool changeableGrav = true;

        [Header("参照用")]
        [SerializeField]
        private AGravBehaviour gravBehaviour;

        [SerializeField]
        private DirectionUIWrapper directionUIWrapper;


        private GravType _targetGravType = GravType.XNegative;

        #region Public Fields

        // チュートリアル用の状態フィールド
        // ターゲットの重力方向を変更したか
        public bool IsTargetGravChanged { get; private set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (gravBehaviour == null)
                Debug.LogError("GravBehaviour is not assigned.");
        }

        private new void Update()
        {
            // 既定を継承しているので、Updateメソッドをオーバーライド
            base.Update();

            // スペースで影響を受けているならフローティングに変換

            if (input.GetMouseButton((int)playerKey.SetObjGravButton, SceneState.InGame)) SetGrav();

            // 右クリックでターゲットの方向を変更
            SetGravDirection();

            // スペースキーでプレイヤーの重力を設定済み方向に変更
            if (input.GetKeyDown(playerKey.SetPlayerGravKey) && changeableGrav)
            {
                var playerVGrav = gravBehaviour as VGravBehaviour;
                if (playerVGrav != null) playerVGrav.SetGravAffected(_targetGravType);
            }

            // カメラに位置を通知
            playerCam.SetPlayerPosAndGrav(transform, gravBehaviour.GravType);
        }

        #endregion

        #region APlayerBehaviour Methods

        protected override Vector3 GetMoveDirection(float deltaTime)
        {
            // WASDキーの入力を取得

            var xInput = input.GetKey(playerKey.MoveForwardKey, SceneState.InGame);
            var zInput = input.GetKey(playerKey.MoveBackwardKey, SceneState.InGame);
            var yInput = input.GetKey(playerKey.MoveLeftKey, SceneState.InGame);
            var wInput = input.GetKey(playerKey.MoveRightKey, SceneState.InGame);

            // 負荷軽減のため、入力がない場合は移動しない
            if (!xInput && !zInput && !yInput && !wInput)
                return Vector3.zero;

            // 入力方向を計算
            var camTransform = playerCam.transform;
            var moveDirection = GravUtils.AdjustDirectionToGrav(
                xInput,
                yInput,
                zInput,
                wInput,
                camTransform.forward,
                camTransform.right,
                gravBehaviour.GravType
            );
            // 移動速度を掛けて、時間を掛ける
            moveDirection *= Speed * deltaTime;

            return moveDirection;
        }

        #endregion

        #region Private Methods

        private void SetGrav()
        {
            // カメラの先のオブジェクトを取得
            var target = playerCam.GetCameraTarget();
            if (target == null)
                return;

            // クリックしたオブジェクトの可変重力コンポーネントを取得
            var targetGravBehaviour = target.GetComponent<VGravBehaviour>();
            if (targetGravBehaviour == null)
                return;

            // ターゲット重力方向にセット
            targetGravBehaviour.SetGravAffected(_targetGravType);
        }

        private void SetGravDirection()
        {
            var method = SettingDataController.Instance.UserSettings.GravSelectMethod;
            if (method == null) return;
            ;

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
                if (!input.GetMouseButton((int)playerKey.ChangeGravDirectionMouseButton, SceneState.InGame))
                    return;

                // カメラの向いている方向を取得
                var camTransform = playerCam.transform;
                var camForward = camTransform.forward;

                // ターゲットの重力方向を変更
                _targetGravType = GravUtils.GetMaxDirection(camForward);

                // ターゲット重力変更済み
                IsTargetGravChanged = true;
            }

            void SetDirectionByKeyboard()
            {
                var modifier = playerKey.ChangeGravDirectionModifierKey;
                if (!input.GetKey(modifier, SceneState.InGame)) return;

                // wasd/space/ctrlでターゲットの重力方向を変更
                // wasdなら移動方向で最も大きい軸を重力方向に設定
                if (
                    input.GetKeyDown(playerKey.MoveForwardKey, SceneState.InGame) ||
                    input.GetKeyDown(playerKey.MoveBackwardKey, SceneState.InGame) ||
                    input.GetKeyDown(playerKey.MoveLeftKey, SceneState.InGame) ||
                    input.GetKeyDown(playerKey.MoveRightKey, SceneState.InGame)
                )
                {
                    // 移動方向から最大軸を取得
                    var moveDirection = GetMoveDirection(1f);
                    _targetGravType = GravUtils.GetMaxDirection(moveDirection);
                }
                else if (input.GetKeyDown(playerKey.ChangeGravDirectionToTopKey, SceneState.InGame))
                {
                    _targetGravType = GravUtils.GetUpperGravType(gravBehaviour.GravType);
                }
                else if (input.GetKeyDown(playerKey.ChangeGravDirectionToBottomKey, SceneState.InGame))
                {
                    _targetGravType = gravBehaviour.GravType;
                }
            }
        }

        #endregion
    }
}