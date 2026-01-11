namespace Lib.State.Scene
{
    public class Setting : ISceneState
    {
        public SceneState StateName => SceneState.Setting;

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