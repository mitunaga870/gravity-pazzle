using System.Collections;
using System.Collections.Generic;
using Behaviour.Camera;
using Behaviour.Gravity;
using Behaviour.Player.Abstract;
using Behaviour.UI;
using Lib.Logic;
using Lib.Logic.Gravity;
using Lib.State.GravAffection;
using Lib.State.Interface.Gravity;
using Lib.State.Player.PlayerGravCtrl;
using UnityEngine;

namespace Behaviour.Player
{
    public class DemoPlayerBehaviour : APlayerBehaviour
    {
        private const float Speed = 5f;

        [SerializeField]
        private VGravBehaviour gravBehaviour;
        [SerializeField]
        private DirectionUIWrapper directionUIWrapper;
        
        private GravType _targetGravType = GravType.XNegative;

        #region Unity Methods
        
        private void Start()
        {
            if (gravBehaviour == null)
            {
                Debug.LogError("GravBehaviour is not assigned.");
                return;
            }
        }
        
        private new void Update()
        {
            // 既定を継承しているので、Updateメソッドをオーバーライド
            base.Update();
            
            // スペースで影響を受けているならフローティングに変換
            if (Input.GetMouseButton(0))
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
            
            // 右クリックでターゲットの方向を変更
            if (Input.GetMouseButton(1))
            {
                // カメラの向いている方向を取得
                var camTransform = playerCam.transform;
                var camForward = camTransform.forward;
                
                // ターゲットの重力方向を変更
                _targetGravType = GravUtils.GetMaxDirection(camForward);
                
                // UIに重力方向を通知
                directionUIWrapper.SetGravType(_targetGravType);
            }
            
            // カメラに位置を通知
            playerCam.SetPlayerPosAndGrav(transform, gravBehaviour.GravType);
        }
        
        #endregion
        
        #region APlayerBehaviour Methods

        protected override Vector3 GetMoveDirection(float deltaTime)
        {
            // WASDキーの入力を取得
            var xInput = Input.GetKey(KeyCode.W);
            var zInput = Input.GetKey(KeyCode.S);
            var yInput = Input.GetKey(KeyCode.A);
            var wInput = Input.GetKey(KeyCode.D);
            
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
    }
}