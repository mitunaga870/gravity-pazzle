using UnityEngine;
using UnityEngine.UI;
using Behaviour.Gravity;

namespace Behaviour.UI
{
    /// <summary>
    /// 重力操作の残り時間を円形ゲージで表示する UI コンポーネント
    /// </summary>
    public class GravityTimerUI : MonoBehaviour
    {
        [SerializeField]
        private Image fillGauge; // Fill Amount で制御するゲージ

        private GravityOperationManager _manager;

        private void Start()
        {
            _manager = GravityOperationManager.Instance;
            
            if (_manager != null)
            {
                // 残り時間割合の変更イベントを購読
                _manager.OnOperationRemainingRatioChanged += OnTimerChanged;
                _manager.OnOperationCountChanged += OnOperationCountChanged;
            }
            else
            {
                Debug.LogWarning($"{nameof(GravityTimerUI)}: GravityOperationManager が見つかりません。");
            }
            
            // 初期状態ではゲージをマックスにする
            if (fillGauge != null)
            {
                fillGauge.fillAmount = 1f;
            }
        }

        private void OnDestroy()
        {
            if (_manager != null)
            {
                _manager.OnOperationRemainingRatioChanged -= OnTimerChanged;
                _manager.OnOperationCountChanged -= OnOperationCountChanged;
            }
        }

        /// <summary>
        /// タイマーの残り割合が変更されたときの処理
        /// </summary>
        private void OnTimerChanged(VGravBehaviour target, float ratio)
        {
            // Fill Amount を更新
            if (fillGauge != null)
            {
                fillGauge.fillAmount = ratio;
            }
        }

        /// <summary>
        /// 操作数が変更されたときの処理
        /// </summary>
        private void OnOperationCountChanged(int activeCount, int maxCount)
        {
            // 操作中でなければゲージをマックスにする
            if (activeCount == 0 && fillGauge != null)
            {
                fillGauge.fillAmount = 1f;
            }
        }
    }
}
