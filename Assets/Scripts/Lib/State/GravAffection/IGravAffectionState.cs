#region

using System.Threading.Tasks;
using Lib.State.Interface.Gravity;

#endregion

namespace Lib.State.GravAffection
{
    public interface IGravAffectionState
    {
        GravAffectionState GetCurrentState { get; }
        GravType GravType { get; }
        public bool Adapting { get; }

        bool Change(IGravAffectionState next, bool forceChange = false);
        
        void OnEnter(IGravAffectionState prev = null);
        Task OnExit(); // 非同期なのでキャンセル処理が必要になるかも
        void OnFixedUpdate();
    }

    public enum GravAffectionState
    {
        Affected,
        Unaffected,
        Floating,
    }
}