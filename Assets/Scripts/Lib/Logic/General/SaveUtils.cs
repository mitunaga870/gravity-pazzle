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

        #region Save

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

        #endregion

        #region Load

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

        #endregion

        #region Delete

        /// <summary>
        ///     セーブデータを削除する
        /// </summary>
        public static void DeleteData(SaveDataType type)
        {
            var savePath = GetSavePath(type);

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log($"Deleted save data at {savePath}");
            }
            else
            {
                Debug.LogWarning($"No save data found to delete at {savePath}");
            }
        }

        /// <summary>
        ///     プレイデータを全て削除する
        /// </summary>
        public static void DeleteAllPlayData()
        {
            var directoryPath = BaseSavePath;

            if (Directory.Exists(directoryPath))
            {
                // 内部のJsonファイルを削除
                foreach (var file in Directory.GetFiles(directoryPath, "*.json"))
                {
                    File.Delete(file);
                    Debug.Log($"Deleted save data at {file}");
                }

                // Settingsフォルダを除外してディレクトリを削除
                foreach (var dir in Directory.GetDirectories(directoryPath))
                {
                    if (Path.GetFileName(dir) == "Settings") continue;

                    Directory.Delete(dir, true);
                    Debug.Log($"Deleted play data directory at {dir}");
                }
            }
            else
                Debug.LogWarning($"No play data directory found to delete at {directoryPath}");
        }

        #endregion

        #region Utils
        /// <summary>
        ///     SaveDataTypeに対応するファイル、ディレクトリのパスを取得する
        /// </summary>
        private static string GetSavePath(SaveDataType type)
        {
            var directoryPath = BaseSavePath;

            var fileName = type switch
            {
                SaveDataType.UserSettings => "Settings/UserSettings.json",
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

        #endregion
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