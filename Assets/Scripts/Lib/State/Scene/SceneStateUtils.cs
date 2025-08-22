namespace Lib.State.Scene
{
    public static class SceneStateUtils
    {
        /// <summary>
        ///     列挙型から状態クラスを作製する
        /// </summary>
        /// <param name="state">状態列挙型</param>
        /// <returns>状態インターフェース</returns>
        public static ISceneState GenerateState(SceneState state)
        {
            return state switch
            {
                SceneState.InGame => new InGame(),
                _ => null
            };
        }
    }
}