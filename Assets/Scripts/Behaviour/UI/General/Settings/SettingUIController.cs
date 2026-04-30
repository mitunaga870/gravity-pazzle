#region

using System;
using System.Collections.Generic;
using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using Lib.DataClass.Settings.GravSelectMethod;
using Lib.Logic.General;
using Lib.State.Scene;
using ScriptableObj.Setting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Resolution = ScriptableObj.Setting.Resolution;

#endregion

namespace Behaviour.UI.General.Settings
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

        [Header("フォーム要素")]
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

        [SerializeField]
        private TMP_Dropdown gravSelectMethodDropdown;

        [SerializeField]
        private Button resetButton;

        [Header("概要用ホバー検知オブジェクト")]
        [SerializeField]
        private DescriptionLinker resolutionLinker;

        [SerializeField]
        private DescriptionLinker masterVolLinker;

        [SerializeField]
        private DescriptionLinker bgmVolLinker;

        [SerializeField]
        private DescriptionLinker seVolLinker;

        [SerializeField]
        private DescriptionLinker tutorialLinker;

        [SerializeField]
        private DescriptionLinker gravSelectMethodLinker;

        [SerializeField]
        private DescriptionLinker resetButtonLinker;

        [Header("概要要素")]
        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private SettingDescription descriptionData;
        
        #endregion

        private static SettingDataController SettingDataController => SettingDataController.Instance;

        private List<Resolution> curResolutions = new();

        private InputController _inputController;

        private SceneStateController _sceneStateController;

        #region Unity Methods

        private void Awake()
        {
            // 各種UI要素のイベントリスナー登録
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChange);
            seVolumeSlider.onValueChanged.AddListener(OnSEVolumeChange);
            tutorialToggle.onValueChanged.AddListener(OnTutorialToggleChange);
            gravSelectMethodDropdown.onValueChanged.AddListener(OnGravSelectMethodChange);
            resetButton.onClick.AddListener(OnResetButtonClick);

            // ドロップダウンの初期化
            SetupResolutionDropdown();
            SetupGravSelectMethodDropdown();

            // 概要のリンク
            resolutionLinker.Setup(descriptionData.ResolutionDescription, descriptionText);
            masterVolLinker.Setup(descriptionData.MasterVolumeDescription, descriptionText);
            bgmVolLinker.Setup(descriptionData.BgmVolumeDescription, descriptionText);
            seVolLinker.Setup(descriptionData.SeVolumeDescription, descriptionText);
            tutorialLinker.Setup(descriptionData.TutorialToggleDescription, descriptionText);
            gravSelectMethodLinker.Setup(descriptionData.GravSelectMethodDescription, descriptionText);
            resetButtonLinker.Setup(descriptionData.ResetDescription, descriptionText);

            _inputController = InputController.Instance;
            _sceneStateController = SceneStateController.Instance;
        }

        private void Update()
        {
            // ESCで閉じる
            if (_inputController.GetKey(KeyCode.Escape, SceneState.Setting))
                HideSettings();
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

        private void OnGravSelectMethodChange(int index)
        {
            var selectedMethod = GravSelectMethodExtensions.Methods[index];
            SettingDataController.SetGravSelectMethod(selectedMethod);
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

        private void SetupGravSelectMethodDropdown()
        {
            gravSelectMethodDropdown.ClearOptions();
            var options = new List<string>();
            var methodList = GravSelectMethodExtensions.Methods;
            foreach (var method in methodList)
                options.Add(method.DisplayName);
            gravSelectMethodDropdown.AddOptions(options);
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

            // 重力選択方法の設定
            var currentGravMethod = Array.FindIndex(
                GravSelectMethodExtensions.Methods,
                method => method.GetType() == SettingDataController.Instance.UserSettings.GravSelectMethod.GetType());
            gravSelectMethodDropdown.value = currentGravMethod;
            gravSelectMethodDropdown.RefreshShownValue();

            // UIに反映
            resolutionDropdown.value = currentResolution;
            resolutionDropdown.RefreshShownValue();
            masterVolumeSlider.value = settings.MasterVolume;
            bgmVolumeSlider.value = settings.BgmVolume;
            seVolumeSlider.value = settings.SeVolume;
            tutorialToggle.isOn = settings.ShowTutorial;
        }

        private void OnResetButtonClick()
        {
            SaveUtils.DeleteAllPlayData();

            SettingDataController.Instance.ReloadAllData();
            PlayerDataController.Instance.ReloadAllData();

            // ステージの場合、保存を無効化する
            var instance = StageDataController.Instance;
            if (instance != null) instance.DontSaveOnDestroy = true;

            StartCoroutine(ReturnToTitleWithSceneTransitionSe());
        }

        #endregion

        #region Public Methods

        private System.Collections.IEnumerator ReturnToTitleWithSceneTransitionSe()
        {
            var soundController = SoundController.Instance;
            if (soundController != null)
            {
                soundController.PlaySe("SceneTransition");
            }

            yield return new WaitForSeconds(SoundController.GetSceneTransitionDelaySeconds());

            SceneManager.LoadScene(SettingDataController.Instance.EnvironmentSetting.TitleScene);
        }

        public void ShowSettings()
        {
            gameObject.SetActive(true);

            // ステート変更
            _sceneStateController.ChangeSceneState(SceneState.Setting);

            // 現在の設定をUIに反映
            LoadCurrentSettings();
        }

        public void HideSettings()
        {
            // ステート変更
            _sceneStateController.ReturnPrevSceneState();
            
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