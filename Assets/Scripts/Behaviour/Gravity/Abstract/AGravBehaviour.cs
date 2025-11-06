#region

using JetBrains.Annotations;
using Lib.State.GravAffection;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Gravity.Abstract
{
    /// <summary>
    /// 重力の影響を受けるオブジェクトの抽象クラス
    /// </summary>
    public abstract class AGravBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GravType initialGravType;
        [SerializeField]
        private Rigidbody affectedRigidBody;
        
        [SerializeField]
        [CanBeNull]
        protected UnityEngine.Camera focusCamera;
        
        protected Rigidbody AffectedRigidBody => affectedRigidBody;

        public GravType GravType =>
            GravAffectionContext == null ? initialGravType : GravAffectionContext.CurrentState.GravType;

        public GravType InitialGravType => initialGravType;
        
        protected GravAffectionContext GravAffectionContext;
        
        protected virtual void Start()
        {
            GravAffectionContext = new GravAffectionContext(
                new GravAffected(
                    initialGravType,
                    affectedRigidBody,
                    focusCamera != null ? focusCamera.transform : null)
                );
        }

        protected abstract void FixedUpdate();

        /// <summary>
        ///     重力状態が適応中かどうか
        /// </summary>
        public bool IsGravAdapting => GravAffectionContext.CurrentState.Adapting;
    }
}