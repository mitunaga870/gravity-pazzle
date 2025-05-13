namespace Lib.State.Player.PlayerGravCtrl
{
    public interface IPlayerGravCtrl
    {
        GravCtrlState GetCurrentState { get; }
        IPlayerGravCtrl PlayerGravCtrl => this;
        
        bool Change(IPlayerGravCtrl next);
        
        void OnEnter(IPlayerGravCtrl prev = null);
        void OnExit();
        void OnFixedUpdate();
    }
    
    public enum GravCtrlState
    {
        Normal,
        Changing,
    }
}