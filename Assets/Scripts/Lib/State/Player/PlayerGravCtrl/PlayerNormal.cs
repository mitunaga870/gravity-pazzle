using Behaviour.Camera;
using Behaviour.Gravity;
using UnityEngine;

namespace Lib.State.Player.PlayerGravCtrl
{
    public class PlayerNormal : IPlayerGravCtrl
    {
        public GravCtrlState GetCurrentState => GravCtrlState.Normal;
        
        public PlayerNormal(
            PlayerGravCtrlContext context,
            VGravBehaviour target,
            PlayerCam playerCam
        ) {
            _target = target;
            _context = context;
            _playerCam = playerCam;
        }
        
        private readonly VGravBehaviour _target;
        private readonly PlayerGravCtrlContext _context;
        private readonly PlayerCam _playerCam;
        
        public bool Change(IPlayerGravCtrl next)
        {
            // 次の状態がNormalでないなら、変更可能
            return next.GetCurrentState != GravCtrlState.Normal;
        }

        public void OnEnter(IPlayerGravCtrl prev = null)
        {
        }

        public void OnExit()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}