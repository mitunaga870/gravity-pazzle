#region

using System;
using Lib.DataClass.PlayData;
using Lib.Logic.General;
using ScriptableObj;
using ScriptableObj.Upgrade;
using UnityEngine;

#endregion

namespace Behaviour.Controller.General.DontDestoroy
{
    public class PlayerDataController : MonoBehaviour
    {
        #region Singleton Implementation

        public static PlayerDataController Instance { get; private set; }

        private PlayerDataController()
        {
        }

        #endregion

        #region Serilized Fields

        [Header("初期セーブデータ")]
        [SerializeField]
        private InitPlayerData initPlayerData;

        [Header("強化情報")]
        [SerializeField]
        private ParamUpgrade operationDurationData;

        [SerializeField]
        private ParamUpgrade maxOperationCountData;

        [SerializeField]
        private ActionUpgrade playerGravChangeData;

        #endregion
        
        #region Data Fields

        public PlayerData PlayerData { get; private set; }

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
            DontDestroyOnLoad(gameObject);

            // プレイヤーデータの初期化
            LoadPlayerData();
        }

        private void OnApplicationQuit()
        {
            // アプリケーション終了時にプレイヤーデータを保存
            SavePlayerData();
        }

        #endregion

        #region Private Methods

        private void LoadPlayerData()
        {
            var isPlayerDataLoaded = SaveUtils.LoadData<PlayerData>(SaveDataType.PlayerData, out var loadedData);
            PlayerData =
                isPlayerDataLoaded ? loadedData : new PlayerData(initPlayerData);
        }

        private void SavePlayerData()
        {
            SaveUtils.SaveData(SaveDataType.PlayerData, PlayerData);
        }

        #endregion

        #region Coin Methods
        
        /// <summary>
        ///     コインを収集したときに呼び出すメソッド
        /// </summary>
        public void CollectCoin(int amount)
        {
            PlayerData = PlayerData.AddCollectedCoinCount(amount);
        }

        #endregion

        #region Upgrade Methods

        public bool Upgrade(UpgradeType type)
        {
            var upgradeData = GetUpgradeData(type);
            var curLevel = PlayerData.GetLevel(type);

            // UIでも隠す予定だが、間違ってアップグレードされないよう処理
            var upgradeable = upgradeData.IsUpgradeable(PlayerData);
            if (!upgradeable) throw new Exception($"{type} is not upgradeable.");

            // コスト確認
            var cost = upgradeData.Cost[curLevel];
            if (PlayerData.CollectedCoinCount < cost) return false;

            // アップグレードの実処理
            if (upgradeData is ParamUpgrade paramUpgrade)
            {
                var nextParam = paramUpgrade.UpgradedParams[curLevel];
                PlayerData = PlayerData.LevelUpParamUpgrade(type, curLevel + 1, nextParam);
            }
            else if (upgradeData is ActionUpgrade actionUpgrade)
            {
                PlayerData = PlayerData.LevelUpActionUpgrade(type);
            }
            else
            {
                throw new Exception($"{type} is not upgradeable.");
            }

            // コイン使用処理
            PlayerData = PlayerData.UseCoin(cost);

            return true;
        }

        public bool IsUpgradeable(UpgradeType type)
        {
            var upgradeData = GetUpgradeData(type);
            return upgradeData.IsUpgradeable(PlayerData);
        }

        private AUpgrade GetUpgradeData(UpgradeType type)
        {
            return type switch
            {
                UpgradeType.OperationDuration => operationDurationData,
                UpgradeType.MaxOperationCount => maxOperationCountData,
                UpgradeType.PlayerGravChange => playerGravChangeData,
                _ => null
            };
        }

        #endregion
        

        /// <summary>
        ///     全てのデータをリロードする
        /// </summary>
        public void ReloadAllData()
        {
            LoadPlayerData();
        }
    }
}