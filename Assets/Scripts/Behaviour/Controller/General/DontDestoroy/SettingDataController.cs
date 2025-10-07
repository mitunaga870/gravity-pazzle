#region

using ScriptableObj;
using UnityEngine;

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

        // ユーザー設定データ
        public int ResolutionWidth { get; private set; }
        public int ResolutionHeight { get; private set; }

        public bool Fullscreen { get; private set; }

        public int TargetDisplay { get; private set; }

        public float MasterVolume { get; private set; }
        public float BgmVolume { get; private set; }
        public float SeVolume { get; private set; }

        public bool ShowTutorial { get; private set; }

        #endregion

        #region Serialized Fields

        [SerializeField]
        private InitSettings initSettings;

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

            loadSettings();
        }

        #endregion

        #region private Method

        private void loadSettings()
        {
            ResolutionWidth = initSettings.ResolutionWidth;
            ResolutionHeight = initSettings.ResolutionHeight;
            Fullscreen = initSettings.Fullscreen;
            TargetDisplay = initSettings.TargetDisplay;
            MasterVolume = initSettings.MasterVolume;
            BgmVolume = initSettings.BgmVolume;
            SeVolume = initSettings.SeVolume;
            ShowTutorial = initSettings.ShowTutorial;
        }

        #endregion
    }
}