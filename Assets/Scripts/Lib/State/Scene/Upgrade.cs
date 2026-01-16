namespace Lib.State.Scene
{
    public class Upgrade : ISceneState
    {
        public SceneState StateName => SceneState.Upgrade;

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