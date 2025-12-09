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
            OperationDurationLevel = 0;
            OperationDuration = initPlayerData.OperationDuration;
            MaxCurrentOperationsLevel = 0;
            MaxCurrentOperations = initPlayerData.MaxConcurrentOperations;
            CanChangePlayerGrav = false;
        }

        [JsonConstructor]
        public PlayerData(
            int collectedCoinCount,
            int operationDurationLevel,
            float operationDuration,
            int maxCurrentOperationsLevel,
            int maxCurrentOperations,
            bool canChangePlayerGrav
        )
        {
            CollectedCoinCount = collectedCoinCount;
            OperationDurationLevel = operationDurationLevel;
            OperationDuration = operationDuration;
            MaxCurrentOperationsLevel = maxCurrentOperationsLevel;
            MaxCurrentOperations = maxCurrentOperations;
            CanChangePlayerGrav = canChangePlayerGrav;
        }

        #endregion

        public readonly int CollectedCoinCount;

        public readonly int OperationDurationLevel;

        public readonly float OperationDuration;

        public readonly int MaxCurrentOperationsLevel;

        public readonly int MaxCurrentOperations;

        public readonly bool CanChangePlayerGrav;

        #region deserver

        private PlayerData DeserveCollectedCoinCount(int collectedCoinCount)
        {
            return new PlayerData(
                collectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                MaxCurrentOperationsLevel,
                MaxCurrentOperations,
                CanChangePlayerGrav
            );
        }

        private PlayerData DeserveOperationDurationLevel(int operationDurationLevel)
        {
            return new PlayerData(
                CollectedCoinCount,
                operationDurationLevel,
                OperationDuration,
                MaxCurrentOperationsLevel,
                MaxCurrentOperations,
                CanChangePlayerGrav
            );
        }

        private PlayerData DeserveOperationDuration(float operationDuration)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                operationDuration,
                MaxCurrentOperationsLevel,
                MaxCurrentOperations,
                CanChangePlayerGrav
            );
        }

        private PlayerData DeserveMaxCurrentOperationsLevel(int maxCurrentOperationsLevel)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                maxCurrentOperationsLevel,
                MaxCurrentOperations,
                CanChangePlayerGrav
            );
        }

        private PlayerData DeserveMaxConcurrentOperations(int maxConcurrentOperations)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                MaxCurrentOperationsLevel,
                maxConcurrentOperations,
                CanChangePlayerGrav
            );
        }

        private PlayerData DeserveCanChangePlayerGrav(bool canChangePlayerGrav)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                MaxCurrentOperationsLevel,
                MaxCurrentOperations,
                canChangePlayerGrav
            );
        }

        #endregion

        public PlayerData AddCollectedCoinCount(int amount)
        {
            return DeserveCollectedCoinCount(CollectedCoinCount + amount);
        }

        public PlayerData LevelUpOperationDuration(int level, float duration)
        {
            var result = DeserveOperationDurationLevel(level);
            result = result.DeserveOperationDuration(duration);

            return result;
        }
    }
}