namespace Lib.State.Scene
{
    public class Instruction : ISceneState
    {
        public SceneState StateName => SceneState.Instruction;

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