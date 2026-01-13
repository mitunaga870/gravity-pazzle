namespace Lib.State.Scene
{
    /// <summary>
    ///     Sceneの状態を表すインターフェース
    /// </summary>
    public interface ISceneState
    {
        /// <summary>
        ///     ステート名
        /// </summary>
        SceneState StateName { get; }

        /// <summary>
        ///   状態を変更できるかどうかをチェックする
        /// </summary>
        /// <param name="next">次の状態</param>
        /// <param name="forceChange">強制的に変更するか</param>
        /// <returns>変更が成功したか</returns>
        bool Changeable(ISceneState next, bool forceChange = false);

        /// <summary>
        ///     状態に入るときの処理
        /// </summary>
        void OnEnter(ISceneState prev = null);

        /// <summary>
        ///     状態から出るときの処理
        /// </summary>
        void OnExit();
    }

    public enum SceneState
    {
        InGame,
        Pause,
        Instruction,
        Setting
    }
}