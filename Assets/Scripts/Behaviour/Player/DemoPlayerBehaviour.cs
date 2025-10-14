#region

using Behaviour.Controller.General;
using Behaviour.Gravity;
using Behaviour.Gravity.Abstract;
using Behaviour.Player.Abstract;
using Behaviour.UI;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using Lib.State.Scene;
using UnityEngine;

#endregion

namespace Behaviour.Player
{
    /// <summary>
    ///     デモプレイヤー用の挙動クラス
    ///     プレイヤー移動とカメラへのプレイヤー位置の通知を行う
    /// </summary>
    public class DemoPlayerBehaviour : APlayerBehaviour
    {
        private const float Speed = 5f;

        [SerializeField]
        private AGravBehaviour gravBehaviour;
        [SerializeField]
        private DirectionUIWrapper directionUIWrapper;
        [SerializeField]
        private InputController inputController;
        
        private GravType _targetGravType = GravType.XNegative;

        #region Public Fields


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

            if (inputController.GetMouseButton(0, SceneState.InGame))
            {
                SetGrav();
            }
            
            // 右クリックでターゲットの方向を変更
            if (inputController.GetMouseButton(1, SceneState.InGame))
            {
                // カメラの向いている方向を取得
                var camTransform = playerCam.transform;
                var camForward = camTransform.forward;
                
                // ターゲットの重力方向を変更
                _targetGravType = GravUtils.GetMaxDirection(camForward);
                
                // UIに重力方向を通知
                directionUIWrapper.SetGravType(_targetGravType);

                // ターゲット重力変更済み
                IsTargetGravChanged = true;
            }
            
            // スペースキーでプレイヤーの重力を設定済み方向に変更
            if (inputController.GetKeyDown(KeyCode.Space))
            {
                var playerVGrav = gravBehaviour as VGravBehaviour;
                if (playerVGrav != null)
                {
                    playerVGrav.SetGravAffected(_targetGravType, false);
                }
            }
            
            // カメラに位置を通知
            playerCam.SetPlayerPosAndGrav(transform, gravBehaviour.GravType);
        }
        
        #endregion
        
        #region APlayerBehaviour Methods

        protected override Vector3 GetMoveDirection(float deltaTime)
        {
            // WASDキーの入力を取得

            var xInput = inputController.GetKey(KeyCode.W, SceneState.InGame);
            var zInput = inputController.GetKey(KeyCode.S, SceneState.InGame);
            var yInput = inputController.GetKey(KeyCode.A, SceneState.InGame);
            var wInput = inputController.GetKey(KeyCode.D, SceneState.InGame);
            
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
        #endregion
    }
}