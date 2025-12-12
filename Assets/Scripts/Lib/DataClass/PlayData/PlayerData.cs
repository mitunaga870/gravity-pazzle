#region

using System;
using Lib.DataClass.Interface;
using Newtonsoft.Json;
using ScriptableObj;
using ScriptableObj.Upgrade;

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
            MaxOperationCountLevel = 0;
            MaxCurrentOperations = initPlayerData.MaxConcurrentOperations;
            PlayerGravChangeLevel = 0;
        }

        [JsonConstructor]
        public PlayerData(
            int collectedCoinCount,
            int operationDurationLevel,
            float operationDuration,
            int maxOperationCountLevel,
            int maxCurrentOperations,
            int playerGravChangeLevel
        )
        {
            CollectedCoinCount = collectedCoinCount;
            OperationDurationLevel = operationDurationLevel;
            OperationDuration = operationDuration;
            MaxOperationCountLevel = maxOperationCountLevel;
            MaxCurrentOperations = maxCurrentOperations;
            PlayerGravChangeLevel = playerGravChangeLevel;
        }

        #endregion

        public readonly int CollectedCoinCount;

        public readonly int OperationDurationLevel;

        public readonly float OperationDuration;

        public readonly int MaxOperationCountLevel;

        public readonly int MaxCurrentOperations;

        public readonly int PlayerGravChangeLevel;

        #region deserver

        private PlayerData DeserveCollectedCoinCount(int collectedCoinCount)
        {
            return new PlayerData(
                collectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                MaxOperationCountLevel,
                MaxCurrentOperations,
                PlayerGravChangeLevel
            );
        }

        private PlayerData DeserveOperationDurationLevel(int operationDurationLevel)
        {
            return new PlayerData(
                CollectedCoinCount,
                operationDurationLevel,
                OperationDuration,
                MaxOperationCountLevel,
                MaxCurrentOperations,
                PlayerGravChangeLevel
            );
        }

        private PlayerData DeserveOperationDuration(float operationDuration)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                operationDuration,
                MaxOperationCountLevel,
                MaxCurrentOperations,
                PlayerGravChangeLevel
            );
        }

        private PlayerData DeserveMaxOperationCountLevel(int maxCurrentOperationsLevel)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                maxCurrentOperationsLevel,
                MaxCurrentOperations,
                PlayerGravChangeLevel
            );
        }

        private PlayerData DeserveMaxOperationCount(int maxConcurrentOperations)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                MaxOperationCountLevel,
                maxConcurrentOperations,
                PlayerGravChangeLevel
            );
        }

        private PlayerData DeservePlayerGravChangeLevel(int canChangePlayerGrav)
        {
            return new PlayerData(
                CollectedCoinCount,
                OperationDurationLevel,
                OperationDuration,
                MaxOperationCountLevel,
                MaxCurrentOperations,
                canChangePlayerGrav
            );
        }

        #endregion

        public PlayerData AddCollectedCoinCount(int amount)
        {
            return DeserveCollectedCoinCount(CollectedCoinCount + amount);
        }

        public PlayerData UseCoin(int amount)
        {
            return DeserveCollectedCoinCount(CollectedCoinCount - amount);
        }

        public PlayerData LevelUpParamUpgrade(UpgradeType type, int nextLevel, float nextParam)
        {
            PlayerData nextPlayerData;

            switch (type)
            {
                case UpgradeType.OperationDuration:
                    nextPlayerData = DeserveOperationDurationLevel(nextLevel);
                    nextPlayerData = nextPlayerData.DeserveOperationDuration(nextParam);
                    break;
                case UpgradeType.MaxOperationCount:
                    nextPlayerData = DeserveMaxOperationCountLevel(nextLevel);
                    nextPlayerData = nextPlayerData.DeserveMaxOperationCount((int)nextParam);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return nextPlayerData;
        }

        public PlayerData LevelUpActionUpgrade(UpgradeType type)
        {
            PlayerData nextPlayerData;

            switch (type)
            {
                case UpgradeType.PlayerGravChange:
                    nextPlayerData = DeservePlayerGravChangeLevel(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return nextPlayerData;
        }

        public int GetLevel(UpgradeType type)
        {
            return type switch
            {
                UpgradeType.OperationDuration => OperationDurationLevel,
                UpgradeType.MaxOperationCount => MaxOperationCountLevel,
                UpgradeType.PlayerGravChange => PlayerGravChangeLevel,
                _ => throw new Exception("対応していないアップグレードタイプです。PlayerData.GetLevel")
            };
        }
    }
}