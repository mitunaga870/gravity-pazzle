#region

using Lib.DataClass.Interface;
using Newtonsoft.Json;
using ScriptableObj;

#endregion

namespace Lib.DataClass.PlayData
{
    /// <summary>
    ///     プレイヤーデータクラス
    /// </summary>
    public class PlayerData : SavableData
    {
        #region Constructor

        public PlayerData(
            InitPlayerData initPlayerData
        )
        {
            CollectedCoinCount = 0;
            OperationDuration = initPlayerData.OperationDuration;
            MaxConcurrentOperations = initPlayerData.MaxConcurrentOperations;
            CanChangePlayerGrav = false;
        }

        [JsonConstructor]
        public PlayerData(
            int collectedCoinCount,
            float operationDuration,
            int maxConcurrentOperations,
            bool canChangePlayerGrav
        )
        {
            CollectedCoinCount = collectedCoinCount;
            OperationDuration = operationDuration;
            MaxConcurrentOperations = maxConcurrentOperations;
            CanChangePlayerGrav = canChangePlayerGrav;
        }

        #endregion

        public readonly int CollectedCoinCount;

        public readonly float OperationDuration;

        public readonly int MaxConcurrentOperations;

        public readonly bool CanChangePlayerGrav;

        #region deserver

        private PlayerData DeserveCollectedCoinCount(int collectedCoinCount)
        {
            return new PlayerData(collectedCoinCount, OperationDuration, MaxConcurrentOperations, CanChangePlayerGrav);
        }

        private PlayerData DeserveOperationDuration(float operationDuration)
        {
            return new PlayerData(CollectedCoinCount, operationDuration, MaxConcurrentOperations, CanChangePlayerGrav);
        }

        private PlayerData DeserveMaxConcurrentOperations(int maxConcurrentOperations)
        {
            return new PlayerData(CollectedCoinCount, OperationDuration, maxConcurrentOperations, CanChangePlayerGrav);
        }

        private PlayerData DeserveCanChangePlayerGrav(bool canChangePlayerGrav)
        {
            return new PlayerData(CollectedCoinCount, OperationDuration, MaxConcurrentOperations, canChangePlayerGrav);
        }

        #endregion

        public PlayerData AddCollectedCoinCount(int amount)
        {
            return DeserveCollectedCoinCount(CollectedCoinCount + amount);
        }
    }
}