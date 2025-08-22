#region

using Behaviour.Controller.General;
using Lib.State.Scene;
using UnityEngine;

#endregion

namespace Behaviour.UI.PauseMenu
{
    /// <summary>
    ///     ポーズメニューのコントローラー。
    ///     表示非表示当を制御する。
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private InputController Input;

        [SerializeField]
        private SceneStateController sceneStateController;
        
        public bool IsMenuOpened { get; private set; }

        private void Start()
        {
            // SerializeFieldが設定されていない場合はエラーを出す
            if (pauseMenu == null)
                Debug.LogError("PauseMenu is not assigned in the inspector.");
            if (Input == null)
                Debug.LogError("InputController is not assigned in the inspector.");
            if (sceneStateController == null)
                Debug.LogError("SceneStateController is not assigned in the inspector.");
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            // Escキーが押されたらポーズメニューをトグル
            if (pauseMenu.activeSelf)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        /// <summary>
        ///     ポーズメニューを表示する。
        /// </summary>
        private void ShowPauseMenu()
        {
            pauseMenu.SetActive(true);

            // カーソルを表示し、ゲームの入力を無効にする
            Cursor.lockState = CursorLockMode.None;
            // カーソルを表示する
            Cursor.visible = true;
            
            // 開いたフラグを立てる
            IsMenuOpened = true;

            // ゲーム状態をポーズに変更
            sceneStateController.ChangeSceneState(SceneState.Pause);
        }

        /// <summary>
        ///     ポーズメニューを非表示にする。
        /// </summary>
        private void HidePauseMenu()
        {
            pauseMenu.SetActive(false);

            // カーソルをロックし、ゲームの入力を有効にする
            Cursor.lockState = CursorLockMode.Locked;
            // カーソルを非表示にする
            Cursor.visible = false;

            // 員ゲーム状態を通常に戻す
            sceneStateController.ChangeSceneState(SceneState.InGame);
        }
    }
}