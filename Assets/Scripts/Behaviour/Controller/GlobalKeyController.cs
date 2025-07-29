#region

using System;
using Behaviour.ObjectFeature;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.Controller
{
    /// <summary>
    /// グローバルなキー操作を管理するクラス
    /// Rを押すと、全てのオブジェクトを初期位置に戻す
    /// </summary>
    public class GlobalKeyController: MonoBehaviour
    {
        private ResetableObject[] _resetableObjects;

        // チュートリアル用の常態
        // リセットが呼ばれた
        public bool IsResetCalled { get; private set; }

        // ハードリセットが呼ばれた
        public bool IsHardResetCalled { get; private set; }
        
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
                (Input.GetKey(KeyCode.LeftShift) 
                 || Input.GetKey(KeyCode.RightShift))
                 && Input.GetKeyDown(KeyCode.R))
            {
                // シーン再読み込み
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                IsHardResetCalled = true;
            }
            // Rキーが押されたら、全てのResetableObjectを初期位置に戻す
            else if (Input.GetKeyDown(KeyCode.R))
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