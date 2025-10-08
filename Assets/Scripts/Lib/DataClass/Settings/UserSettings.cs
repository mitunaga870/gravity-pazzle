#region

using System;
using Newtonsoft.Json;
using ScriptableObj;
using UnityEngine;

#endregion

namespace Lib.DataClass.Settings
{
    /// <summary>
    ///     ユーザー設定のデータクラス
    /// </summary>
    [Serializable]
    public class UserSettings
    {
        #region Constructors

        public UserSettings(InitUserSettings initUserSettings)
        {
            ResolutionWidth = initUserSettings.ResolutionWidth;
            ResolutionHeight = initUserSettings.ResolutionHeight;
            Fullscreen = initUserSettings.Fullscreen;
            MasterVolume = initUserSettings.MasterVolume;
            BgmVolume = initUserSettings.BgmVolume;
            SeVolume = initUserSettings.SeVolume;
            ShowTutorial = initUserSettings.ShowTutorial;
        }

        [JsonConstructor]
        public UserSettings(int initUserSettings, int height, FullScreenMode fullscreen, float masterVolume,
            float bgmVolume, float seVolume, bool showTutorial)
        {
            // 引数で受け取った値をセットする
            ResolutionWidth = initUserSettings;
            ResolutionHeight = height;
            Fullscreen = fullscreen;
            MasterVolume = masterVolume;
            BgmVolume = bgmVolume;
            SeVolume = seVolume;
            ShowTutorial = showTutorial;
        }

        #endregion

        public readonly int ResolutionWidth;
        public readonly int ResolutionHeight;
        public readonly FullScreenMode Fullscreen;

        public readonly float MasterVolume;
        public readonly float BgmVolume;
        public readonly float SeVolume;

        public readonly bool ShowTutorial;

        #region deserver

        public UserSettings DeserveResolution(int width, int height, FullScreenMode fullscreenArg)
        {
            return new UserSettings(
                width,
                height,
                fullscreenArg,
                MasterVolume,
                BgmVolume,
                SeVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveBgmVolume(float bgmVolumeArg)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                MasterVolume,
                bgmVolumeArg,
                SeVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveSeVolume(float seVolumeArg)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                MasterVolume,
                BgmVolume,
                seVolumeArg,
                ShowTutorial
            );
        }

        public UserSettings DeserveMasterVolume(float masterVolumeArg)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                masterVolumeArg,
                BgmVolume,
                SeVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveShowTutorial(bool showTutorialArg)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                MasterVolume,
                BgmVolume,
                SeVolume,
                showTutorialArg
            );
        }

        #endregion

        #region Parse

        public override string ToString()
        {
            // デバッグ用    
            return
                $"Resolution: {ResolutionWidth}x{ResolutionHeight} Fullscreen: {Fullscreen} " +
                $"MasterVolume: {MasterVolume} BgmVolume: {BgmVolume} SeVolume: {SeVolume} " +
                $"ShowTutorial: {ShowTutorial}";
        }

        /// <summary>
        ///     保存用のJSON文字列に変換する
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}