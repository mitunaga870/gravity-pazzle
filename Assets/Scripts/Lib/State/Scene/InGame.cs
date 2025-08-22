namespace Lib.State.Scene
{
    public class InGame : ISceneState
    {
        public SceneState StateName => SceneState.InGame;

        public bool Change(ISceneState next, bool forceChange = false)
        {
            return true;
        }

        public void OnEnter(ISceneState prev = null)
        {
        }

        public void OnExit()
        {
        }
    }
}