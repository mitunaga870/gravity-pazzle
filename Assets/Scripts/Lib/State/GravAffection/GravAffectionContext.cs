using UnityEngine;

namespace Lib.State.GravAffection
{
    public class GravAffectionContext
    {
        IGravAffectionState _prevState;
        
        public IGravAffectionState CurrentState { get; private set; }

        public GravAffectionContext(
            IGravAffectionState initialState,
            Camera followCamera = null
        ) {
            CurrentState = initialState;
            
            _prevState = null;
            
            CurrentState?.OnEnter();
        }

        public bool SetState(IGravAffectionState next)
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