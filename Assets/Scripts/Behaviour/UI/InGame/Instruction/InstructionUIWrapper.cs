#region

using System;
using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
using Lib.State.Scene;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.InGame.Instruction
{
    /// <summary>
    ///     操作指南を表示するUIのラッパークラス
    /// </summary>
    public class InstructionUIWrapper : MonoBehaviour
    {
        private int currentPageIndex;

        #region SerializeField

        [SerializeField]
        private Image[] pageImages;

        [SerializeField]
        private SceneStateController sceneStateController;

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            // ステートをゲーム中に変更する
            sceneStateController.ChangeSceneState(SceneState.InGame);
        }

        private void Start()
        {
            // チュートリアルの有効確認
            var userSetting = SettingDataController.Instance.UserSettings;
            if (!userSetting.ShowTutorial)
            {
                gameObject.SetActive(false);
                return;
            }

            // pageImagesが設定されているか確認する
            if (pageImages == null || pageImages.Length == 0)
                throw new NullReferenceException("Page images are not assigned.");

            // ステートを合わせる
            sceneStateController.ChangeSceneState(SceneState.Instruction);

            // 最初のページを表示する
            ShowPage(0);
        }

        private void Update()
        {
            // 何等かのキーが押されたら次のページへ進む
            if (!Input.anyKeyDown) return;
            currentPageIndex++;
            if (currentPageIndex >= pageImages.Length)
            {
                // 全ページ表示し終わったらUIを閉じる
                gameObject.SetActive(false);
                return;
            }

            // 次のページを表示する
            ShowPage(currentPageIndex);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     指定したページを表示する
        /// </summary>
        /// <param name="pageIndex">ページ番号</param>
        private void ShowPage(int pageIndex)
        {
            for (var i = 0; i < pageImages.Length; i++)
                pageImages[i].gameObject.SetActive(i == pageIndex);
        }

        #endregion
    }
}