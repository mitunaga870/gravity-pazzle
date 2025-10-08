#region

using System.Collections.Generic;
using Behaviour.Controller.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Resolution = ScriptableObj.Setting.Resolution;

#endregion

namespace Behaviour.UI.Settings
{
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

        #region Unity Methods

        private void Start()
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
            var selectedResolution = resolutions[index];

            Debug.Log($"解像度が変更されました: {selectedResolution}");
            SettingDataController.SetResolution(
                selectedResolution.Width,
                selectedResolution.Height,
                selectedResolution.FullscreenMode);
        }

        private void OnMasterVolumeChange(float value)
        {
            // マスターボリュームの変更処理をここに実装
            Debug.Log($"マスターボリュームが変更されました: {value}");

            SettingDataController.SetMasterVolume(value);
        }

        private void OnBGMVolumeChange(float value)
        {
            // BGMボリュームの変更処理をここに実装
            Debug.Log($"BGMボリュームが変更されました: {value}");

            SettingDataController.SetBgmVolume(value);
        }

        private void OnSEVolumeChange(float value)
        {
            // SEボリュームの変更処理をここに実装
            Debug.Log($"SEボリュームが変更されました: {value}");

            SettingDataController.SetSeVolume(value);
        }

        private void OnTutorialToggleChange(bool isOn)
        {
            // チュートリアルのオンオフ切り替え処理をここに実装
            Debug.Log($"チュートリアルの設定が変更されました: {isOn}");

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

        #endregion 
    }
}