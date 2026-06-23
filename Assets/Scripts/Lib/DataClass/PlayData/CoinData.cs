#region

using System.Collections.Immutable;
using System.Linq;
using Lib.DataClass.Interface;
using Lib.Logic.General;
using Newtonsoft.Json;

#endregion

namespace Lib.DataClass.PlayData
{
    /// <summary>
    ///     プレイ状況の保存データクラス
    /// </summary>
    public class CoinData : SavableData
    {
        #region Constructors

        public CoinData(string stageId)
        {
            StageId = stageId;

            // セーブデータから既存のコインデータを読み込む
            var isLoaded = SaveUtils.LoadStageData<CoinData>(StageId, StageSaveDataType.CoinData, out var loadedData);
            CollectedCoinIds = isLoaded ? loadedData.CollectedCoinIds : ImmutableHashSet<string>.Empty;
        }

        [JsonConstructor]
        public CoinData(string stageId, string[] collectedCoinIds)
        {
            StageId = stageId;
            CollectedCoinIds = collectedCoinIds is { Length: > 0 }
                ? collectedCoinIds.ToImmutableHashSet()
                : ImmutableHashSet<string>.Empty;
        }

        #endregion

        #region Public Fields

        // 取得したコインの識別子を保持するセット
        [JsonIgnore]
        public readonly ImmutableHashSet<string> CollectedCoinIds;

        // 情報を保持しているステージID
        public readonly string StageId;

        #endregion

        #region Serialization

        [JsonProperty("CollectedCoinIds")]
        private string[] CollectedCoinIdsForJson => CollectedCoinIds.ToArray();

        #endregion

        #region deserver

        private CoinData DeserveCollectedCoinIds(ImmutableHashSet<string> collectedCoinIds)
        {
            return new CoinData(StageId, collectedCoinIds.ToArray());
        }

        #endregion


        /// <summary>
        ///     コインを取得する
        /// </summary>
        /// <param name="coinId">コインの識別子</param>
        public CoinData CollectCoin(string coinId)
        {
            // コインIDを取得済みセットに追加（重複登録を防止）
            var collectedCoinIds = CollectedCoinIds.Add(coinId);
            return DeserveCollectedCoinIds(collectedCoinIds);
        }

        /// <summary>
        ///     取得したコインの数を取得
        /// </summary>
        public int CollectedCoinCount => CollectedCoinIds.Count;

        /// <summary>
        ///     特定のコインが取得済みかどうかを確認
        /// </summary>
        /// <param name="coinId">コインの識別子</param>
        /// <returns>取得済みの場合true</returns>
        public bool IsCoinCollected(string coinId)
        {
            return CollectedCoinIds.Contains(coinId);
        }
    }
}
