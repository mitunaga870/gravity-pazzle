#region

using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature
{
    /// <summary>
    ///     オブジェクトを特定のターゲットに追従させるクラス
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class FollowObj : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (target != null)
                _rigidbody.MovePosition(target.position);
        }
    }
}