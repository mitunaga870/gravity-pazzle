namespace Lib.State.Scene
{
    public class Pause : ISceneState
    {
        public SceneState StateName => SceneState.Pause;

        public bool Changeable(ISceneState next, bool forceChange = false)
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