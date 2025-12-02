#region

using Newtonsoft.Json;

#endregion

namespace Lib.DataClass.Interface
{
    /// <summary>
    ///     セーブの為にJSON化可能なデータクラスのインターフェース
    /// </summary>
    public class SavableData
    {
        /// <summary>
        ///     データクラスをJSON文字列に変換する
        /// </summary>
        public string ToJson()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            return JsonConvert.SerializeObject(this, serializerSettings);
        }
    }
}