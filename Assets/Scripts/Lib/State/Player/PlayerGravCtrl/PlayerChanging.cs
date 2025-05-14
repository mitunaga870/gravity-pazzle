using Behaviour.Camera;
using Behaviour.Gravity;
using Lib.Logic;
using Lib.Logic.Gravity;
using UnityEngine;

namespace Lib.State.Player.PlayerGravCtrl
{
    public class PlayerChanging : IPlayerGravCtrl
    {
        public GravCtrlState GetCurrentState => GravCtrlState.Changing;

        public PlayerChanging(
            PlayerGravCtrlContext context,
            VGravBehaviour playerGravBehaviour,
            VGravBehaviour target,
            PlayerCam playerCam
        ) {
            _target = target;
            _playerCam = playerCam;
            _context = context;
            _playerGravBehaviour = playerGravBehaviour;
        }
        
        private readonly VGravBehaviour _target; 
        private readonly PlayerCam _playerCam;
        private readonly PlayerGravCtrlContext _context;
        private readonly VGravBehaviour _playerGravBehaviour;
        
        private bool _isChangeable = false;

        public bool Change(IPlayerGravCtrl next)
        {
            return next.GetCurrentState != GravCtrlState.Changing;
        }

        public void OnEnter(IPlayerGravCtrl prev = null)
        {
            _target.SetGravFloating();
            
            // 0.5s後操作可能に
            var coroutine = GeneralUtils.DelayCoroutine(0.5f,() =>
            {
                _isChangeable = true;
            });
            _target.StartCoroutine(coroutine);
        }

        public void OnExit()
        {
            
        }

        public void OnFixedUpdate()
        {
            if (!_isChangeable)
                return;
            
            // スペース・シフトを優先
            if (Input.GetKey(KeyCode.Space))
            {
                var gravType = GravUtils.GetUpperGravType(_playerGravBehaviour.GravType);
                _target.SetGravAffected(gravType);
                
                // 待機に遷移
                _context.SetState(new PlayerNormal(_context, _target, _playerCam));
                
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                var gravType = GravUtils.GetDownGravType(_playerGravBehaviour.GravType);
                _target.SetGravAffected(gravType);
                
                // 待機に遷移
                _context.SetState(new PlayerNormal(_context, _target, _playerCam));
                
                return;
            }
            
            // WASDで重力設定
            // まず、今回の重力状況とWASD入力から現在の入力ベクトルを取得
            var camTransform = _playerCam.transform;
            var inputVector = GravUtils.AdjustDirectionToGrav(
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.D),
                camTransform.forward,
                camTransform.right,
                _playerGravBehaviour.GravType
            );
            // 0なら破棄
            if (inputVector.sqrMagnitude <= 0)
                return;
            
            // 最大入力ベクトル方向に重力を設定
            var newGravType = GravUtils.GetMaxDirection(inputVector);
            
            // 重力を設定
            _target.SetGravAffected(newGravType);
            
            // 変更中モードに遷移
            _context.SetState(new PlayerNormal(_context, _target, _playerCam));
        }
    }
}