#region

using UnityEngine;

#endregion

namespace Lib.State.GravAffection
{
    /// <summary>
    ///     重力の影響ステートを管理するクラス
    /// </summary>
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

        /// <summary>
        ///     次の重力状態を設定します。
        /// </summary>
        /// <param name="next"></param>
        /// <returns> できたかどうかをboolで </returns>
        public bool SetState(IGravAffectionState next, bool forceChange = false)
        {
            if (!CurrentState.Change(next, forceChange))
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