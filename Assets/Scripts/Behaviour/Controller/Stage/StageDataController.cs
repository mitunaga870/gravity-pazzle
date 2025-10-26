#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Behaviour.Controller.Stage
{
    /// <summary>
    ///     ステージデータ管理クラス
    ///     シングルトンパターンで実装予定
    /// </summary>
    public class StageDataController : MonoBehaviour
    {
        #region Singleton Implementation

        public static StageDataController Instance { get; private set; }

        private StageDataController()
        {
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion

        #region Private Variables

        private readonly List<int> _nonReactiveBodyIds = new();
        private Rigidbody _playerRigidbody;

        #endregion

        #region Accessors

        public int[] NonReactiveBodyIds => _nonReactiveBodyIds.ToArray();

        /// <summary>
        ///     反作用を起こさないオブジェクトのIDリストにIDを追加する
        /// </summary>
        /// <param name="id">追加するRigidbodyのInstanceID</param>
        public void AddPlayerBodyInstanceID(int id)
        {
            if (!_nonReactiveBodyIds.Contains(id))
                _nonReactiveBodyIds.Add(id);
        }

        public Rigidbody PlayerRigidbody
        {
            get => _playerRigidbody;
            set
            {
                if (_playerRigidbody == null)
                    _playerRigidbody = value;
                else
                    Debug.LogWarning("PlayerRigidbody has already been set and cannot be changed.");
            }
        }

        #endregion
    }
}