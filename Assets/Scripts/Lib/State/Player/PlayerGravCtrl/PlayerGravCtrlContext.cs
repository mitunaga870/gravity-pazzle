namespace Lib.State.Player.PlayerGravCtrl
{
    public class PlayerGravCtrlContext
    {
        private IPlayerGravCtrl _prevState;
        public IPlayerGravCtrl CurrentState { get; private set; }

        public PlayerGravCtrlContext(
            IPlayerGravCtrl initialState
        ) {
            CurrentState = initialState;
            
            _prevState = null;
            
            CurrentState?.OnEnter();
        }

        public bool SetState(IPlayerGravCtrl next)
        {
            if(!CurrentState.Change(next))
                return false;
            
            //　状態更新
            _prevState = CurrentState;
            CurrentState = next;
            
            // 前の状態を終了
            _prevState?.OnExit();
            // 新しい状態を開始
            CurrentState?.OnEnter(_prevState);
            
            return true;
        }

        public void OnFixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }
    }
}