#region

using Lib.DataClass.Interface;
using Newtonsoft.Json;

#endregion

namespace Lib.DataClass.PlayData
{
    /// <summary>
    ///     プレイヤーデータクラス
    /// </summary>
    public class PlayerData : SavableData
    {
        #region Constructor

        public PlayerData() : this(0)
        {
        }

        [JsonConstructor]
        public PlayerData(int collectedCoinCount)
        {
            CollectedCoinCount = collectedCoinCount;
        }

        #endregion

        public readonly int CollectedCoinCount;

        #region deserver

        private PlayerData DeserveCollectedCoinCount(int collectedCoinCount)
        {
            return new PlayerData(collectedCoinCount);
        }

        #endregion

        public PlayerData AddCollectedCoinCount(int amount)
        {
            return DeserveCollectedCoinCount(CollectedCoinCount + amount);
        }
    }
}