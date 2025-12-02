#region

using System;
using Lib.DataClass.Settings;
using Lib.DataClass.Settings.GravSelectMethod;
using Lib.Logic.General;
using ScriptableObj;
using ScriptableObj.Setting;
using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace Behaviour.Controller.General.DontDestoroy
{
    /// <summary>
    ///     ユーザー設定情報の管理を行うコントローラー
    ///     シングルトンパターンで実装予定
    /// </summary>
    [Serializable]
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

        public EnvironmentSetting EnvironmentSetting => environmentSetting;

        public PlayerKey PlayerKey => playerKey;

        #endregion

        #region Serialized Fields

        [SerializeField]
        private InitUserSettings initUserSettings;

        [SerializeField]
        private EnvironmentSetting environmentSetting;

        [SerializeField]
        private PlayerKey playerKey;

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
        }

        private void OnDestroy()
        {
            // 多重生成を防ぐ
            if (Instance != this) return;

            // 設定を保存する
            SaveSettings();
            Instance = null;
        }

        #endregion

        #region private Method

        private void LoadSettings()
        {
            // セーブデータの読み込みを試みる
            if (SaveUtils.LoadData<UserSettings>(SaveDataType.UserSettings, out var userSettings))
            {
                UserSettings = userSettings;
            }
            else
            {
                ResetSettings();
                
                Debug.Log($"Loaded default UserSettings: {UserSettings}");
            }
        }

        /**
         * 設定を保存する
         */
        private void SaveSettings()
        {
            // セーブデータを保存する
            SaveUtils.SaveData(SaveDataType.UserSettings, UserSettings);
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
        ///     重力方向選択方法を設定する
        /// </summary>
        public void SetGravSelectMethod(IGravSelectMethod selectedMethod)
        {
            UserSettings = UserSettings.DeserveGravSelectMethod(selectedMethod);
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
            
            var masterVolume = UserSettings.MasterVolume <= 0.0001f ? -80f : Mathf.Log10(UserSettings.MasterVolume) * 20;
            var bgmVolume = UserSettings.BgmVolume <= 0.0001f ? -80f : Mathf.Log10(UserSettings.BgmVolume) * 20;
            var seVolume = UserSettings.SeVolume <= 0.0001f ? -80f : Mathf.Log10(UserSettings.SeVolume) * 20;

            audioMixer.SetFloat("MasterVolume", masterVolume);
            audioMixer.SetFloat("BgmVolume", bgmVolume);
            audioMixer.SetFloat("SeVolume", seVolume);
        }

        #endregion
    }
}