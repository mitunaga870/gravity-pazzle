#region

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
        }

        /// <summary>
        ///     ポーズメニューを非表示にする。
        /// </summary>
        private void HidePauseMenu()
        {
            pauseMenu.SetActive(false);

            // カーソルをロックし、ゲームの入力を有効にする
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}