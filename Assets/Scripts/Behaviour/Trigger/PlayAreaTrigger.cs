using Behaviour.ObjectFeature;
using UnityEngine;

namespace Behaviour.Trigger
{
    /// <summary>
    /// 外に出て1秒後にリセットするトリガー
    /// </summary>
    public class PlayAreaTrigger: MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            // 出たのがプレイヤーでなければ無視
            if (!other.CompareTag("Player")) return;

            Debug.Log("VAR");
            
            // 全てのResetableObjectを初期位置に戻す
            var resetableObjects = FindObjectsOfType<ResetableObject>();
            foreach (var resetableObject in resetableObjects)
            {
                resetableObject.ResetPosition();
            }
        }
    }
}