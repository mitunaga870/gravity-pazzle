using Behaviour.Gravity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviour.UI
{
    /// <summary>
    /// 重力操作の残り可能回数を表示するUIコンポーネント
    /// テキスト表示とストック画像の両方で視覚的に表現する
    /// </summary>
    public class GravityOperationCountUI : MonoBehaviour
    {
        [Header("テキスト表示")]
        [SerializeField]
        private TextMeshProUGUI countText;

        [Header("ストック画像")]
        [SerializeField]
        private Image[] stockImages;

        [Header("ストックスプライト")]
        [SerializeField]
        private Sprite stockFullSprite;

        [SerializeField]
        private Sprite stockEmptySprite;

        private void Start()
        {
            var manager = GravityOperationManager.Instance;

            // 操作数変更イベントを購読
            manager.OnOperationCountChanged += OnOperationCountChanged;

            // 初期状態を設定
            UpdateUI(0, manager.MaxConcurrentOperations);
        }

        /// <summary>
        /// 操作数が変更されたときの処理
        /// </summary>
        private void OnOperationCountChanged(int activeCount, int maxCount)
        {
            UpdateUI(activeCount, maxCount);
        }

        /// <summary>
        /// UIを更新する
        /// </summary>
        private void UpdateUI(int activeCount, int maxCount)
        {
            // テキスト表示を更新
            var remainingCount = maxCount - activeCount;
            countText.text = $"{remainingCount}/{maxCount}";

            // ストック画像を更新
            for (int i = 0; i < stockImages.Length; i++)
            {
                // インデックスが残り個数未満なら満タン、それ以上なら空
                if (i < remainingCount)
                {
                    stockImages[i].sprite = stockFullSprite; // 満タン
                }
                else
                {
                    stockImages[i].sprite = stockEmptySprite; // 空
                }
            }
        }
    }
}
