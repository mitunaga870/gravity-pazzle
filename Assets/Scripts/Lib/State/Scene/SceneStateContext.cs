namespace Lib.State.Scene
{
    /// <summary>
    ///     Sceneの状態を管理するコンテキストクラス
    /// </summary>
    public class SceneStateContext
    {
        /// <summary>
        ///     現在の状態を取得
        /// </summary>
        public ISceneState CurrentState { get; private set; }

        /// <summary>
        ///     初期化
        /// </summary>
        /// <param name="initialState"></param>
        public SceneStateContext(ISceneState initialState)
        {
            CurrentState = initialState;
            CurrentState?.OnEnter();
        }

        /// <summary>
        ///     状態を変更する
        /// </summary>
        /// <param name="next">次の状態</param>
        /// <param name="forceChange">強制的に変更するか</param>
        /// <returns>変更が成功したか</returns>
        public bool Change(ISceneState next, bool forceChange = false)
        {
            if (CurrentState != null && !CurrentState.Changeable(next, forceChange)) return false;

            CurrentState?.OnExit();
            var previous = CurrentState;
            CurrentState = next;
            CurrentState.OnEnter(previous);

            return true;
        }
    }
}