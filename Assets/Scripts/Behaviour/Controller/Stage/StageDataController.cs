#region

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

        private int _playerBodyInstanceID = -1;
        private Rigidbody _playerRigidbody;

        #endregion

        #region Accessors

        public int PlayerBodyInstanceID
        {
            get => _playerBodyInstanceID;
            set
            {
                if (_playerBodyInstanceID == -1)
                    _playerBodyInstanceID = value;
                else
                    Debug.LogWarning("PlayerInstanceID has already been set and cannot be changed.");
            }
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