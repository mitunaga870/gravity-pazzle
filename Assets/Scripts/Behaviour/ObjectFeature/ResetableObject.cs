#region

using Behaviour.Camera;
using Behaviour.Gravity;
using Lib.Logic;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature
{
    /// <summary>
    /// 初期位置を覚え、初期位置に戻す機能を持つオブジェクト
    /// </summary>
    public class ResetableObject : MonoBehaviour
    {
        private Vector3 _initialPosition;

        private bool _hasRigidbody;
        private Rigidbody _rigidbody;

        private bool _hasGravBehaviour;
        private VGravBehaviour _gravBehaviour;

        private bool _hasPlayerCam;
        private PlayerCam _playerCam;

        // リセットのための一時状況を解除させるための遅延時間
        private const float ResetDelay = 0.1f;

        private void Awake()
        {
            // 初期位置と必要なコンポーネントをキャッシュする
            _initialPosition = transform.position;

            _rigidbody = GetComponent<Rigidbody>();
            _hasRigidbody = _rigidbody != null;

            _gravBehaviour = GetComponent<VGravBehaviour>();
            _hasGravBehaviour = _gravBehaviour != null;

            _playerCam = GetComponent<PlayerCam>();
            _hasPlayerCam = _playerCam != null;
        }

        /// <summary>
        ///     位置をリセットする
        /// </summary>
        public void ResetPosition()
        {
            EnsureGameObjectIsActive();
            ResetRigidbodyState();
            ResetGravityState();
            ResetPlayerCamState();

            // 物理演算が位置変更に干渉しないように、Rigidbodyをkinematicにした後で位置をリセットすることが重要
            transform.position = _initialPosition;
        }

        #region Private Methods

        private void EnsureGameObjectIsActive()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        private void ResetRigidbodyState()
        {
            if (!_hasRigidbody) return;

            // 位置をクリーンにリセットし、既存の動きを停止させるために、一時的にRigidbodyをkinematicにする
            _rigidbody.isKinematic = true;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            // 少し遅延させてからRigidbodyのkinematicを解除する
            var coroutine =
                GeneralUtils.DelayCoroutine(ResetDelay, () => { _rigidbody.isKinematic = false; });
            StartCoroutine(coroutine);
        }


        private void ResetGravityState()
        {
            if (!_hasGravBehaviour) return;

            // オブジェクトの重力を初期設定に戻す
            _gravBehaviour.SetGravAffected(_gravBehaviour.InitialGravType, true, false);
        }

        private void ResetPlayerCamState()
        {
            if (!_hasPlayerCam) return;

            // リセット時の移動量を移動しないようにする
            var currentPos = transform.position;
            _playerCam.OffsetNextFollow += _initialPosition - currentPos;
        }

        #endregion
    }
}