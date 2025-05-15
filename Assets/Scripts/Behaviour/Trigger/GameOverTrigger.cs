using System;
using Behaviour.ObjectFeature;
using UnityEngine;

namespace Behaviour.Trigger
{
    public class GameOverTrigger : MonoBehaviour
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
        private void OnTriggerEnter(Collider other)
        {
            // プレイヤーが触れた場合の処理
            if (other.CompareTag("Player"))
            {
                // ゲームオーバーの処理を追加することができます
                Debug.Log("Game Over!");

                // 全てのResetableObjectを初期位置に戻す
                ResetAllObjects();
            }
        }

        #endregion
        
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