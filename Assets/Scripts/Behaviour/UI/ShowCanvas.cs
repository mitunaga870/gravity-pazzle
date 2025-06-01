#region

using UnityEngine;

#endregion

namespace Behaviour.UI
{
    /// <summary>
    /// トリガー領域内にプレイヤーがいる間、
    /// 指定の Canvas（または UI）を表示し、
    /// 領域外では非表示にします。
    /// </summary>
    public class ShowCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject _canvasObject;  // 表示/非表示する UI の GameObject

        private void Awake()
        {
            // 初期状態で非アクティブにしておく
            if (_canvasObject != null)
                _canvasObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _canvasObject != null)
                _canvasObject.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && _canvasObject != null)
                _canvasObject.SetActive(false);
        }
    }
}
