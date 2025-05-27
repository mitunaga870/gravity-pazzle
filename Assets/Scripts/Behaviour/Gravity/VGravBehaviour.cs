#region

using Behaviour.Gravity.Abstract;
using Lib.State.GravAffection;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Gravity
{
    /// <summary>
    /// 可変重力の挙動を持つオブジェクトのクラス
    /// </summary>
    public class VGravBehaviour: AGravBehaviour
    {

        private bool _isFocusCameraNotNull;
        
        public IGravAffectionState CurrentGravState => GravAffectionContext.CurrentState;
        
        # region Unity Methods

        protected override void Start()
        {
            _isFocusCameraNotNull = focusCamera != null;
            
            base.Start();
        }

        protected override void FixedUpdate()
        {
            // キーが押されたら重力の向きを変える
            GravAffectionContext.OnFixedUpdate();
        }
        
        #endregion
        
        #region Public Methods

        public void SetGravFloating()
        {
            if (
                !GravAffectionContext.
                    SetState(
                        new GravFloating(
                            GravType, 
                            AffectedRigidBody,
                            transform,
                            _isFocusCameraNotNull ? focusCamera!.transform : null
            )))
                Debug.LogError("Failed to set GravFloating state.");
        }

        /// <summary>
        ///     重力を設定方向にする
        /// </summary>
        /// <param name="gravType">指定する重力を指定していする</param>
        /// <param name="forceChange">速度があるときに強制的に変更するかどうか</param>
        public void SetGravAffected(GravType gravType, bool forceChange = false)
        {
            if (
                !GravAffectionContext.
                    SetState(
                        new GravAffected(
                            gravType, 
                            AffectedRigidBody,
                            _isFocusCameraNotNull ? focusCamera!.transform : null),
                        forceChange
                    ))
                Debug.Log("Failed to set GravAffected state.");
        }
        
        #endregion
    }
}