#region

using UnityEngine;

#endregion

namespace Behaviour.Player
{
    /// <summary>
    ///     プレイヤーのアニメーションを操作するスクリプト
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimBehaviour : MonoBehaviour
    {
        private Animator _animator;

        #region Animator Hash

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
        }

        #endregion

        /// <summary>
        ///     移動速度を更新
        /// </summary>
        public void UpdateSpeed(float speed)
        {
            _animator.SetFloat(SpeedHash, speed);
        }
    }
}