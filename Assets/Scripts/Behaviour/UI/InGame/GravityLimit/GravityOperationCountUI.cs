#region

using Behaviour.Gravity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.InGame.GravityLimit
{
    /// <summary>
    /// 重力操作の残り可能回数を表示するUIコンポーネント
    /// テキスト表示とストック画像の両方で視覚的に表現する
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class GravityOperationCountUI : MonoBehaviour
    {
        [Header("テキスト表示")]
        [SerializeField]
        private TextMeshProUGUI countText;

        [Header("ストック画像プレファブ")]
        [SerializeField]
        private Image stockPrefab;

        [Header("ストックスプライト")]
        [SerializeField]
        private Sprite stockFullSprite;

        [SerializeField]
        private Sprite stockEmptySprite;

        private Image[] _stockImages;
        

        private void Start()
        {
            var manager = GravityOperationManager.Instance;

            // 操作数変更イベントを購読
            manager.OnOperationCountChanged += OnOperationCountChanged;

            // 初期状態を設定
            Initialize(manager.MaxConcurrentOperations);
            UpdateUI(0, manager.MaxConcurrentOperations);
        }

        private void Initialize(int max)
        {
            // 既存スプライト削除
            if (_stockImages != null)
                foreach (var stockSprite in _stockImages)
                    Destroy(stockSprite);

            // スプライト配列作成
            _stockImages = new Image[max];
            for (var i = 0; i < max; i++)
                _stockImages[i] = Instantiate(stockPrefab, transform);
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
            if (maxCount != _stockImages.Length)
                Initialize(maxCount);
            
            // テキスト表示を更新
            var remainingCount = maxCount - activeCount;
            countText.text = $"{remainingCount}/{maxCount}";
            
            // ストック画像を更新
            for (var i = 0; i < _stockImages.Length; i++)
            {
                // インデックスが残り個数未満なら満タン、それ以上なら空
                if (i < remainingCount)
                {
                    _stockImages[i].sprite = stockFullSprite; // 満タン
                }
                else
                {
                    _stockImages[i].sprite = stockEmptySprite; // 空
                }
            }
        }
    }
}
