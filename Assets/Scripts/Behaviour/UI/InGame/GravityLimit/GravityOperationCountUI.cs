#region

using Behaviour.Gravity;
using System.Collections.Generic;
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

        [Header("ストック画像のコンテナ")]
        [SerializeField]
        private Transform stockContainer;

        private readonly List<Image> _stockImages = new List<Image>();

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
            // 残り操作可能数を計算
            var remainingCount = maxCount - activeCount;
            
            // テキスト表示を更新
            countText.text = $"{remainingCount}/{maxCount}";
            
            // 現在の画像数と残り操作可能数の差分を調整
            while (_stockImages.Count < remainingCount)
            {
                // 足りない分を追加
                var parent = stockContainer != null ? stockContainer : transform;
                var newStock = Instantiate(stockPrefab, parent);
                _stockImages.Add(newStock);
            }
            
            while (_stockImages.Count > remainingCount)
            {
                // 多い分を削除
                var lastIndex = _stockImages.Count - 1;
                Destroy(_stockImages[lastIndex].gameObject);
                _stockImages.RemoveAt(lastIndex);
            }
        }
    }
}
