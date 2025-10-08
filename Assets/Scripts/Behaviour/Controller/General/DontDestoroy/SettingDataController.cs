#region

using Lib.DataClass.Settings;
using ScriptableObj;
using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace Behaviour.Controller.General
{
    /// <summary>
    ///     ユーザー設定情報の管理を行うコントローラー
    ///     シングルトンパターンで実装予定
    /// </summary>
    public class SettingDataController : MonoBehaviour
    {
        #region Singleton Implementation

        public static SettingDataController Instance { get; private set; }


        private SettingDataController()
        {
        }

        #endregion

        #region Data Fields

        private UserSettings _userSettings;

        public UserSettings UserSettings
        {
            get => _userSettings;

            private set
            {
                if (_userSettings != null && _userSettings.Equals(value)) return;

                _userSettings = value;
                // 変更があった場合に設定を適用する
                ApplySettings();
            }
        }

        #endregion

        #region Serialized Fields

        [SerializeField]
        private InitUserSettings initUserSettings;

        [SerializeField]
        private AudioMixer audioMixer;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // 多重生成を防ぐ
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // シングルトンパターンの実装
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();

            ApplySettings();
        }

        #endregion

        #region private Method

        private void LoadSettings()
        {
            ResetSettings();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     解像度を設定する
        /// </summary>
        public void SetResolution(int width, int height, FullScreenMode fullscreen)
        {
            width = Mathf.Max(800, width);
            height = Mathf.Max(600, height);

            UserSettings = UserSettings.DeserveResolution(width, height, fullscreen);
        }

        /// <summary>
        ///     BGMボリュームを設定する
        /// </summary>
        public void SetBgmVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);

            UserSettings = UserSettings.DeserveBgmVolume(volume);
        }

        /// <summary>
        ///     SEボリュームを設定する
        /// </summary>
        public void SetSeVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            UserSettings = UserSettings.DeserveSeVolume(volume);
        }

        /// <summary>
        ///     マスターボリュームを設定する
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            UserSettings = UserSettings.DeserveMasterVolume(volume);
        }

        /// <summary>
        ///     チュートリアルの表示設定を変更する
        /// </summary>
        public void SetShowTutorial(bool showTutorial)
        {
            UserSettings = UserSettings.DeserveShowTutorial(showTutorial);
        }

        /// <summary>
        ///     設定内容を初期化する
        /// </summary>
        public void ResetSettings()
        {
            UserSettings = new UserSettings(initUserSettings);
        }

        /// <summary>
        ///     設定を反映する
        /// </summary>
        public void ApplySettings()
        {
            Screen.SetResolution(
                UserSettings.ResolutionWidth,
                UserSettings.ResolutionHeight,
                UserSettings.Fullscreen
            );

            audioMixer.SetFloat("MasterVolume", Mathf.Log10(UserSettings.MasterVolume) * 20);
            audioMixer.SetFloat("BgmVolume", Mathf.Log10(UserSettings.BgmVolume) * 20);
            audioMixer.SetFloat("SeVolume", Mathf.Log10(UserSettings.SeVolume) * 20);
        }

        #endregion
    }
}