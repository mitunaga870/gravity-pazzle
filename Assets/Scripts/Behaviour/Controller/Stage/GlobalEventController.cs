#region

using System;
using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.ObjectFeature;
using Lib.State.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.Controller
{
    /// <summary>
    /// グローバルなイベントを管理するクラス
    /// Rを押すと、全てのオブジェクトを初期位置に戻す
    /// </summary>
    public class GlobalEventController : MonoBehaviour
    {
        private ResetableObject[] _resetableObjects;

        // チュートリアル用の状態
        // リセットが呼ばれた
        public bool IsResetCalled { get; private set; }

        // ハードリセットが呼ばれた
        public bool IsHardResetCalled { get; private set; }

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        private InputController Input;
        
        # region Unity Methods
        [Obsolete("Obsolete")]
        private void Start()
        {
            // シーン内の全てのResetableObjectを取得
            _resetableObjects = FindObjectsOfType<ResetableObject>();
        }
        
        [Obsolete("Obsolete")]
        private void Update()
        {
            
            // Shift+Rキーが押されたら、全てのResetableObjectを初期位置に戻す
            if (
                (Input.GetKey(KeyCode.LeftShift, SceneState.InGame)
                 || Input.GetKey(KeyCode.RightShift, SceneState.InGame))
                && Input.GetKeyDown(KeyCode.R, SceneState.InGame))
            {
                // シーン再読み込み
                var soundController = SoundController.Instance;
                if (soundController != null)
                {
                    soundController.PlaySe("SceneTransition");
                }

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                IsHardResetCalled = true;
            }
            // Rキーが押されたら、全てのResetableObjectを初期位置に戻す
            else if (Input.GetKeyDown(KeyCode.R, SceneState.InGame))
            {
                ResetAllObjects();
                IsResetCalled = true;
            }
        }
        # endregion
        
        #region Private Methods
        [Obsolete("Obsolete")]
        private void ResetAllObjects()
        {
            // 全てのResetableObjectを初期位置に戻す
            foreach (var resetableObject in _resetableObjects)
            {
                resetableObject.ResetPosition();
            }
        }
        #endregion
    }
}