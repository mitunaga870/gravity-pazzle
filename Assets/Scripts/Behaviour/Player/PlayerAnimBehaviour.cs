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
        private const float AnimationCrossFade = 0.1f;
        
        private Animator _animator;

        private bool _prevFalling;

        #region Animator Hash

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private static readonly int TurnHash = Animator.StringToHash("Turn");

        private static readonly int LocoMotionHash = Animator.StringToHash("LocoMotion");

        private static readonly int FallHash = Animator.StringToHash("Fall");

        private static readonly int LandingHash = Animator.StringToHash("Landing");

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

        public void IsFalling(bool isFalling)
        {
            // 連続でクロスフェードしないようにする
            if (_prevFalling == isFalling) return;
            _prevFalling = isFalling;

            if (isFalling)
                _animator.CrossFade(FallHash, AnimationCrossFade);
            else
                _animator.Play(LocoMotionHash);
        }
    }
}