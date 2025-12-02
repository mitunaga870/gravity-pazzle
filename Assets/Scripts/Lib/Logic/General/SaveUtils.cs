#region

using System;
using System.IO;
using Lib.DataClass.Interface;
using Newtonsoft.Json;
using UnityEngine;

#endregion

namespace Lib.Logic.General
{
    /// <summary>
    ///     セーブデータの保存・読み込み・管理を行うユーティリティクラス
    /// </summary>
    public static class SaveUtils
    {
        private static string BaseSavePath => Application.persistentDataPath;

        /// <summary>
        ///     セーブデータを保存する
        /// </summary>
        public static void SaveData(SaveDataType type, SavableData dataClass)
        {
            var data = dataClass.ToJson();
            var savePath = GetSavePath(type);

            File.WriteAllText(savePath, data);
        }

        /// <summary>
        ///     セーブデータを読み込む
        ///     ファイルが存在しない場合はfalseを返す
        /// </summary>
        public static bool LoadData<T>(SaveDataType type, out T data) where T : SavableData
        {
            var savePath = GetSavePath(type);
            if (!File.Exists(savePath))
            {
                data = null;
                return false;
            }

            try
            {
                var deserializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                var json = File.ReadAllText(savePath);
                data = JsonConvert.DeserializeObject<T>(json, deserializerSettings);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save data from {savePath}: {e.Message}");
                data = null;
                return false;
            }
        }

        /// <summary>
        ///     SaveDataTypeに対応するファイル、ディレクトリのパスを取得する
        /// </summary>
        private static string GetSavePath(SaveDataType type)
        {
            var directoryPath = BaseSavePath;
            string fileName;

            switch (type)
            {
                case SaveDataType.UserSettings:
                    directoryPath += "/Settings";
                    fileName = "UserSettings.json";
                    break;
                case SaveDataType.StarCoinCollection:
                    directoryPath += "/Save/StarCoins";
                    fileName = "StarCoinCollection.json";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            // ディレクトリが存在しない場合は作成する
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, fileName);
        }
    }

    /// <summary>
    ///     セーブデータの種類を表す列挙型
    /// </summary>
    public enum SaveDataType
    {
        UserSettings,
        StarCoinCollection
    }
}