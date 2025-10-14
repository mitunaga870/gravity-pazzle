#region

using System.Collections.Generic;
using Behaviour.Controller.General.DontDestoroy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Resolution = ScriptableObj.Setting.Resolution;

#endregion

namespace Behaviour.UI.Settings
{
    /// <summary>
    ///     設置UIのコントローラー
    ///     UI要素とユーザー設定データの同期を担当
    /// </summary>
    public class SettingUIController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("解像度プリセット")]
        [SerializeField]
        private List<Resolution> resolutions;

        [Header("UI要素")]
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;

        [SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField]
        private Slider bgmVolumeSlider;

        [SerializeField]
        private Slider seVolumeSlider;

        [SerializeField]
        private Toggle tutorialToggle;

        #endregion

        private static SettingDataController SettingDataController => SettingDataController.Instance;

        private List<Resolution> curResolutions = new();

        #region Unity Methods

        private void Awake()
        {
            // 解像度の設定
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
            // マスターボリュームの設定
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
            // BGMボリュームの設定
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChange);
            // SEボリュームの設定
            seVolumeSlider.onValueChanged.AddListener(OnSEVolumeChange);
            // チュートリアルの設定
            tutorialToggle.onValueChanged.AddListener(OnTutorialToggleChange);

            // 解像度ドロップダウンの初期化
            SetupResolutionDropdown();
        }

        #endregion

        #region Handler Methods

        private void OnResolutionChange(int index)
        {
            // 解像度の変更処理をここに実装
            var selectedResolution = curResolutions[index];

            SettingDataController.SetResolution(
                selectedResolution.Width,
                selectedResolution.Height,
                selectedResolution.FullscreenMode);
        }

        private void OnMasterVolumeChange(float value)
        {
            SettingDataController.SetMasterVolume(value);
        }

        private void OnBGMVolumeChange(float value)
        {
            SettingDataController.SetBgmVolume(value);
        }

        private void OnSEVolumeChange(float value)
        {
            SettingDataController.SetSeVolume(value);
        }

        private void OnTutorialToggleChange(bool isOn)
        {
            SettingDataController.SetShowTutorial(isOn);
        }

        #endregion

        #region Private Methods

        private void SetupResolutionDropdown()
        {
            resolutionDropdown.ClearOptions();
            var options = new List<string>();
            foreach (var res in resolutions) options.Add(res.DisplayString);
            resolutionDropdown.AddOptions(options);
        }

        private void LoadCurrentSettings()
        {
            // 値を取得
            var settings = SettingDataController.UserSettings;

            // 現在の解像度からプリセットを探索
            var currentResolution = resolutions.FindIndex(res =>
                res.Width == settings.ResolutionWidth &&
                res.Height == settings.ResolutionHeight &&
                res.FullscreenMode == settings.Fullscreen);


            // 現在の解像度リストを更新
            curResolutions = new List<Resolution>(resolutions);

            // 見つからなかった場合、カスタム現設定を追加
            if (currentResolution == -1)
            {
                // カスタム解像度を作成
                var customResolution = ScriptableObject.CreateInstance<Resolution>();
                customResolution.Init(
                    settings.ResolutionWidth,
                    settings.ResolutionHeight,
                    settings.Fullscreen);

                // 解像度リストに追加
                curResolutions.Add(customResolution);
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("カスタム：" + customResolution.DisplayString));

                // 現在の解像度をカスタムに設定
                currentResolution = resolutions.Count - 1;
            }

            Debug.Log($"Current Resolution Index: {currentResolution}");

            // UIに反映
            resolutionDropdown.value = currentResolution;
            resolutionDropdown.RefreshShownValue();
            masterVolumeSlider.value = settings.MasterVolume;
            bgmVolumeSlider.value = settings.BgmVolume;
            seVolumeSlider.value = settings.SeVolume;
            tutorialToggle.isOn = settings.ShowTutorial;
        }

        #endregion

        #region Public Methods

        public void ShowSettings()
        {
            gameObject.SetActive(true);

            // 現在の設定をUIに反映
            LoadCurrentSettings();
        }

        public void HideSettings()
        {
            gameObject.SetActive(false);
        }

        public void ToggleSettings()
        {
            if (gameObject.activeSelf)
                HideSettings();
            else
                ShowSettings();
        }

        #endregion
    }
}