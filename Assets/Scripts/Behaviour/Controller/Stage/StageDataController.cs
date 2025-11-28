#region

using System;
using System.Collections.Generic;
using Behaviour.Camera;
using Behaviour.Gravity.Abstract;
using Behaviour.Player.Abstract;
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


            // シーン内のPlayerBehaviourを探して設定
            var playerBehaviours = FindObjectsByType<APlayerBehaviour>(FindObjectsSortMode.None);
            PlayerBehaviour = playerBehaviours.Length != 0 ? playerBehaviours[0] : null;
            if (playerBehaviours.Length != 1)
                Debug.LogWarning(
                    $"There should be exactly one PlayerBehaviour in the scene. Found: {playerBehaviours.Length}");
            // PlayerのGravBehaviourを取得
            if (PlayerBehaviour != null) PlayerGravBehaviour = PlayerBehaviour.GetComponent<AGravBehaviour>();

            // シーン内のPlayerCamを探して設定
            var playerCams = FindObjectsByType<PlayerCam>(FindObjectsSortMode.None);
            PlayerCam = playerCams.Length != 0 ? playerCams[0] : null;
            if (playerCams.Length != 1)
                Debug.LogWarning(
                    $"There should be exactly one PlayerCam in the scene. Found: {playerCams.Length}");
            
        }

        #endregion

        #region Private Variables

        private readonly List<int> _nonReactiveBodyIds = new();
        private Rigidbody _playerRigidbody;
        
        private readonly HashSet<string> _allCoinIds = new();
        private readonly HashSet<string> _collectedCoinIds = new();

        #endregion

        #region Public Variables

        public TimeSpan PlayTime { get; set; }

        public APlayerBehaviour PlayerBehaviour { get; private set; }
        public AGravBehaviour PlayerGravBehaviour { get; private set; }

        public PlayerCam PlayerCam { get; private set; }

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

        #region Coin Management

        /// <summary>
        ///     コインを登録する
        /// </summary>
        /// <param name="coinId">コインの識別子</param>
        public void RegisterCoin(string coinId)
        {
            _allCoinIds.Add(coinId);
        }

        /// <summary>
        ///     コインを取得する
        /// </summary>
        /// <param name="coinId">コインの識別子</param>
        public void CollectCoin(string coinId)
        {
            _collectedCoinIds.Add(coinId);
        }

        /// <summary>
        ///     配置されたコインの総数を取得
        /// </summary>
        public int TotalCoinCount => _allCoinIds.Count;

        /// <summary>
        ///     取得したコインの数を取得
        /// </summary>
        public int CollectedCoinCount => _collectedCoinIds.Count;

        /// <summary>
        ///     配置されたすべてのコインの識別子を取得
        /// </summary>
        public IReadOnlyCollection<string> AllCoinIds => _allCoinIds;

        /// <summary>
        ///     取得したコインの識別子を取得
        /// </summary>
        public IReadOnlyCollection<string> CollectedCoinIds => _collectedCoinIds;

        /// <summary>
        ///     特定のコインが取得済みかどうかを確認
        /// </summary>
        /// <param name="coinId">コインの識別子</param>
        /// <returns>取得済みの場合true</returns>
        public bool IsCoinCollected(string coinId)
        {
            return _collectedCoinIds.Contains(coinId);
        }

        #endregion
    }
}