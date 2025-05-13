using System.Collections;
using System.Collections.Generic;
using Behaviour.Camera;
using Behaviour.Gravity;
using Behaviour.Player.Abstract;
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
        [SerializeField]
        private float speed = 5f;
        
        [SerializeField]
        private VGravBehaviour gravBehaviour;
        
        [SerializeField]
        private PlayerCam playerCam;
        
        private Vector3 _prevPos;
        private PlayerGravCtrlContext _playerGravCtrlContext;

        #region Unity Methods
        
        private void Start()
        {
            if (gravBehaviour == null)
            {
                Debug.LogError("GravBehaviour is not assigned.");
                return;
            }
            
            // 初期位置設定
            _prevPos = transform.position;
            
            // コンテキストの初期化
            _playerGravCtrlContext =
                new PlayerGravCtrlContext(
                    new PlayerNormal(
                        _playerGravCtrlContext,
                        gravBehaviour,
                        playerCam
                    ));
        }
        
        private new void Update()
        {
            // 既定を継承しているので、Updateメソッドをオーバーライド
            base.Update();
            
            // プレイヤーの位置をカメラに設定
            // ReSharper disable once InvertIf
            if (_prevPos != transform.position)
            {
                var position = transform.position;
                playerCam.SetPlayerPosAndGrav(position, gravBehaviour.GravType);
                _prevPos = position;
            }
            
            // スペースで影響を受けているならフローティングに変換
            if (Input.GetKeyDown(KeyCode.Space) && _playerGravCtrlContext.CurrentState.GetCurrentState == GravCtrlState.Normal) {
                
                // 変更中モードに遷移
                _playerGravCtrlContext.SetState(
                    new PlayerChanging(_playerGravCtrlContext, gravBehaviour, playerCam)
                    );
            } 
            else if (Input.GetMouseButton(0) && _playerGravCtrlContext.CurrentState.GetCurrentState == GravCtrlState.Normal)
            {
                // カメラの先のオブジェクトを取得
                var target = playerCam.GetCameraTarget();
                if (target == null)
                    return;
                
                // クリックしたオブジェクトの可変重力コンポーネントを取得
                var targetGravBehaviour = target.GetComponent<VGravBehaviour>();
                if (targetGravBehaviour == null)
                    return;
                
                // 待機に遷移
                _playerGravCtrlContext.SetState(
                    new PlayerChanging(_playerGravCtrlContext, targetGravBehaviour, playerCam
                    ));
            }
        }
        
        private void FixedUpdate()
        {
            // プレイヤーの重力状態を更新
            _playerGravCtrlContext.OnFixedUpdate();
            Debug.Log(_playerGravCtrlContext.CurrentState.GetCurrentState);
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
            moveDirection *= speed * deltaTime;
            
            return moveDirection;
        }
        
        #endregion
    }
}