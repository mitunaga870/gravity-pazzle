#region

using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using Behaviour.UI.General.Settings;
using Lib.State.Scene;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.InGame.PauseMenu
{
    /// <summary>
    ///     ポーズメニューのコントローラー。
    ///     表示非表示当を制御する。
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private FonMover pauseMenu;

        [SerializeField]
        private SceneSelectButton goToTitleButton;

        [SerializeField]
        private Button goToSettingsButton;

        [SerializeField]
        private SceneSelectButton quitStageButton;

        [Header("シーンない参照")]
        [SerializeField]
        private InputController Input;

        [SerializeField]
        private SceneStateController sceneStateController;
        
        [SerializeField]
        private SettingUIController settingUIController;

        #endregion
        
        public bool IsMenuOpened { get; private set; }

        private CursorLockMode _previousCursorLockMode;
        private bool _previousCursorVisibility;

        private bool _isOpen;

        private void Start()
        {
            // SerializeFieldが設定されていない場合はエラーを出す
            if (pauseMenu == null)
                Debug.LogError("PauseMenu is not assigned in the inspector.");
            if (Input == null)
                Debug.LogError("InputController is not assigned in the inspector.");
            if (sceneStateController == null)
                Debug.LogError("SceneStateController is not assigned in the inspector.");

            // ボタンアクションの設定
            goToTitleButton.SetTargetScene(
                SettingDataController.Instance.EnvironmentSetting.TitleScene);
            quitStageButton.SetTargetScene(
                SettingDataController.Instance.EnvironmentSetting.StageSelectScene);
            goToSettingsButton.onClick.AddListener(() =>
                settingUIController.ShowSettings());

            // ステージじゃない場合はあきらめるボタンを非表示
            var stageDataController = FindObjectsByType<StageDataController>(FindObjectsSortMode.None);
            if (stageDataController.Length == 0) quitStageButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            // Escキーが押されたらポーズメニューをトグル
            if (_isOpen)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        /// <summary>
        ///     ポーズメニューを表示する。
        /// </summary>
        private void ShowPauseMenu()
        {
            pauseMenu.reverse = false;
            pauseMenu.PlayMotion();

            // カーソルの状態を保存
            _previousCursorLockMode = Cursor.lockState;
            _previousCursorVisibility = Cursor.visible;
            // カーソルを解放し、ゲームの入力を無効にする
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // 開いたフラグを立てる
            IsMenuOpened = true;
            _isOpen = true;

            // ゲーム状態をポーズに変更
            sceneStateController.ChangeSceneState(SceneState.Pause);
        }

        /// <summary>
        ///     ポーズメニューを非表示にする。
        /// </summary>
        private void HidePauseMenu()
        {
            pauseMenu.reverse = true;
            pauseMenu.PlayMotion();

            // カーソルの状態を元に戻す
            Cursor.lockState = _previousCursorLockMode;
            Cursor.visible = _previousCursorVisibility;

            // 開いたフラグを下ろす
            _isOpen = false;

            // ゲーム状態を通常に戻す
            sceneStateController.ChangeSceneState(SceneState.InGame);
        }
    }
}