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
        private Image fillGauge; // Fill Amountで制御するゲージ

        private GravityOperationManager _manager;

        private void Start()
        {
            _manager = GravityOperationManager.Instance;
            
            // 残り時間割合の変更イベントを購読
            _manager.OnOperationRemainingRatioChanged += OnTimerChanged;
            _manager.OnOperationCountChanged += OnOperationCountChanged;
            
            // 初期状態ではゲージをマックスにする
            fillGauge.fillAmount = 1f;
        }

        /// <summary>
        /// タイマーの残り割合が変更されたときの処理
        /// </summary>
        private void OnTimerChanged(VGravBehaviour target, float ratio)
        {
            // Fill Amount を更新
            fillGauge.fillAmount = ratio;
        }

        /// <summary>
        /// 操作数が変更されたときの処理
        /// </summary>
        private void OnOperationCountChanged(int activeCount, int maxCount)
        {
            // 操作中でなければゲージをマックスにする
            if (activeCount == 0)
            {
                fillGauge.fillAmount = 1f;
            }
        }
    }
}
