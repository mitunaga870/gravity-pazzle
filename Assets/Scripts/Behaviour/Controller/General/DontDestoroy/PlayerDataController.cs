#region

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

        [Header("強化後パラメータ")]
        [SerializeField]
        private UpgradedOperationTime upgradedOperationTime;

        [SerializeField]
        private UpgradedMaxOperationCount upgradedMaxOperationCount;

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

        /// <summary>
        ///     コインを収集したときに呼び出すメソッド
        /// </summary>
        public void CollectCoin(int amount)
        {
            PlayerData = PlayerData.AddCollectedCoinCount(amount);
        }


        /// <summary>
        ///     全てのデータをリロードする
        /// </summary>
        public void ReloadAllData()
        {
            LoadPlayerData();
        }
    }
}