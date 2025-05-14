using System;
using Behaviour.ObjectFeature;
using UnityEngine;

namespace Behaviour.Controller
{
    /// <summary>
    /// グローバルなキー操作を管理するクラス
    /// Rを押すと、全てのオブジェクトを初期位置に戻す
    /// </summary>
    public class GlobalKeyController: MonoBehaviour
    {
        private ResetableObject[] _resetableObjects;
        
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
            // Rキーが押されたら、全てのResetableObjectを初期位置に戻す
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetAllObjects();
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