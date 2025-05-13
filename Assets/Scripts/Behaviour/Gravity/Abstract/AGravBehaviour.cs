using JetBrains.Annotations;
using Lib.State.GravAffection;
using Lib.State.Interface.Gravity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviour.Gravity.Abstract
{
    /// <summary>
    /// 重力の影響を受けるオブジェクトの抽象クラス
    /// </summary>
    public abstract class AGravBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody affectedRigidBody;
        
        [SerializeField]
        [CanBeNull]
        protected UnityEngine.Camera focusCamera;
        
        protected Rigidbody AffectedRigidBody => affectedRigidBody;
        
        public GravType GravType => GravAffectionContext.CurrentState.GravType;
        
        protected GravAffectionContext GravAffectionContext;
        
        protected virtual void Start()
        {
            GravAffectionContext = new GravAffectionContext(
                new GravAffected(
                    GravType.YNegative,
                    affectedRigidBody,
                    focusCamera != null ? focusCamera.transform : null)
                );
        }

        protected abstract void FixedUpdate();
    }
}