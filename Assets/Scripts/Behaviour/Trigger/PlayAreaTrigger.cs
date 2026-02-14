using Behaviour.ObjectFeature;
using UnityEngine;

namespace Behaviour.Trigger
{
    /// <summary>
    /// 1秒間外に出るとリセットするトリガー
    /// </summary>
    public class PlayAreaTrigger: MonoBehaviour
    {
        private bool _isPlayerInside = false;
        
        private float _exitTime;
        
        private void OnTriggerEnter(Collider other)
        {
            // 入ったのがプレイヤーでなければ無視
            if (!other.CompareTag("Player")) return;

            _isPlayerInside = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            // 出たのがプレイヤーでなければ無視
            if (!other.CompareTag("Player")) return;
            
            _isPlayerInside = false;
        }
        
        private void Update()
        {
            if (_isPlayerInside)
            {
                // プレイヤーが中にいる場合、退出時間をリセット
                _exitTime = Time.time;
            }
            else
            {
                // プレイヤーが外にいる場合、1秒経過したらリセット
                if (Time.time - _exitTime >= 1.0f)
                {
                    ResetAllPosition();
                    _exitTime = Time.time; // リセット後、再度カウントを開始
                }
            }
        }
        
        private void ResetAllPosition()
        {
            var resetableObjects = FindObjectsOfType<ResetableObject>();
            
            foreach (var obj in resetableObjects)
            {
                obj.ResetPosition();
            }
        }
    }
}