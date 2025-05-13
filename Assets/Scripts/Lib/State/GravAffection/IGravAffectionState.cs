using Behaviour.Gravity.Abstract;
using Lib.State.Interface.Gravity;
using UnityEngine;

namespace Lib.State.GravAffection
{
    public interface IGravAffectionState
    {
        GravAffectionState GetCurrentState { get; }
        GravType GravType { get; }
        
        bool Change(IGravAffectionState next);
        
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