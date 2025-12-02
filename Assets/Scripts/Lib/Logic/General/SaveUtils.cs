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

            Debug.Log($"Saving data to {savePath}, data: {data}");

            File.WriteAllText(savePath, data);
        }

        /// <summary>
        ///     ステージ固有のセーブデータを保存する
        /// </summary>
        public static void SaveStageData(string stageId, StageSaveDataType type, SavableData dataClass)
        {
            var data = dataClass.ToJson();
            var savePath = GetStageSavePath(stageId, type);

            Debug.Log($"Saving stage data to {savePath}, data: {data}");

            File.WriteAllText(savePath, data);
        }

        /// <summary>
        ///     セーブデータを読み込む
        ///     ファイルが存在しない場合はfalseを返す
        /// </summary>
        public static bool LoadData<T>(SaveDataType type, out T data) where T : SavableData
        {
            var savePath = GetSavePath(type);

            return LoadExe(savePath, out data);
        }


        /// <summary>
        ///     ステージ固有のセーブデータを読み込む
        ///     ファイルが存在しない場合はfalseを返す
        /// </summary>
        /// <param name="stageId"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public static bool LoadStageData<T>(string stageId, StageSaveDataType type, out T data) where T : SavableData
        {
            var savePath = GetStageSavePath(stageId, type);

            return LoadExe(savePath, out data);
        }

        /// <summary>
        ///     ロードの実体処理
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static bool LoadExe<T>(string path, out T data) where T : SavableData
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Save file not found at {path}");
                
                data = null;
                return false;
            }

            try
            {
                var deserializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                var json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<T>(json, deserializerSettings);

                Debug.Log($"Loaded save data from {path}: {json}, data: {data}");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save data from {path}: {e.Message}");
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

            var fileName = type switch
            {
                SaveDataType.UserSettings => "UserSettings.json",
                SaveDataType.PlayerData => "PlayerData.json",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            // ディレクトリが存在しない場合は作成する
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, fileName);
        }

        /// <summary>
        ///     StageSaveDataTypeに対応するファイル、ディレクトリのパスを取得する
        /// </summary>
        private static string GetStageSavePath(string stageId, StageSaveDataType type)
        {
            var directoryPath = BaseSavePath + "/Stages/" + stageId;

            var fileName = type switch
            {
                StageSaveDataType.CoinData => "CoinData.json",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

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
        PlayerData
    }

    /// <summary>
    ///     ステージ固有のセーブデータの種類を表す列挙型
    /// </summary>
    public enum StageSaveDataType
    {
        CoinData
    }
}