#region

using Lib.State.Interface.Gravity;

#endregion

namespace Lib.State.GravAffection
{
    public interface IGravAffectionState
    {
        GravAffectionState GetCurrentState { get; }
        GravType GravType { get; }

        bool Change(IGravAffectionState next, bool forceChange = false);
        
        void OnEnter(IGravAffectionState prev = null);
        void OnExit();
        void OnFixedUpdate();
    }

    public enum GravAffectionState
    {
        Affected,
        Unaffected,
        Floating,
    }
}