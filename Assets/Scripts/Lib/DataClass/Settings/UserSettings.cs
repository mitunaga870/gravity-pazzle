#region

using ScriptableObj;
using UnityEngine;

#endregion

namespace Lib.DataClass.Settings
{
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

        private UserSettings(int initUserSettings, int height, FullScreenMode fullscreen, float masterVolume,
            float bgmVolume, float seVolume, bool showTutorial)
        {
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

        public UserSettings DeserveResolution(int width, int height, FullScreenMode fullscreen)
        {
            return new UserSettings(
                width,
                height,
                fullscreen,
                MasterVolume,
                BgmVolume,
                SeVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveBgmVolume(float bgmVolume)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                MasterVolume,
                bgmVolume,
                SeVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveSeVolume(float seVolume)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                MasterVolume,
                BgmVolume,
                seVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveMasterVolume(float masterVolume)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                masterVolume,
                BgmVolume,
                SeVolume,
                ShowTutorial
            );
        }

        public UserSettings DeserveShowTutorial(bool showTutorial)
        {
            return new UserSettings(
                ResolutionWidth,
                ResolutionHeight,
                Fullscreen,
                MasterVolume,
                BgmVolume,
                SeVolume,
                showTutorial
            );
        }

        #endregion
    }
}