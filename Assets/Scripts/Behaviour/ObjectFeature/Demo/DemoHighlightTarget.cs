using Behaviour.UI.General;
using UnityEngine;

namespace Behaviour.ObjectFeature.Demo
{
    /// <summary>
    /// テスト用：このコンポーネントを付けたオブジェクトを <see cref="HighlightController"/> のハイライト対象にする。
    /// </summary>
    public class DemoHighlightTarget : MonoBehaviour
    {
        private void OnEnable()
        {
            RegisterAsHighlightTarget();
        }

        private void Start()
        {
            // HighlightController が後から有効化されると OnEnable 時点では Instance が無いことがあるため再試行する
            RegisterAsHighlightTarget();
        }

        private void RegisterAsHighlightTarget()
        {
            if (HighlightController.Instance == null)
            {
                Debug.LogWarning($"{nameof(DemoHighlightTarget)}: HighlightController.Instance が未初期化です。", this);
                return;
            }

            HighlightController.Instance.SetHighlight(gameObject);
        }

        private void OnDestroy()
        {
            if (HighlightController.Instance != null)
                HighlightController.Instance.ClearHighlightIfCurrent();
        }

        private void OnDisable()
        {
            if (HighlightController.Instance != null)
                HighlightController.Instance.ClearHighlightIfCurrent();
        }
    }
}
