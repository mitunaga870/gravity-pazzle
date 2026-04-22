using UnityEngine;
using UnityEngine.UI;
using Behaviour.Gravity;
using Behaviour.Controller.General.DontDestoroy;
using ScriptableObj.Upgrade;

namespace Behaviour.UI
{
    /// <summary>
    /// 重力操作の残り時間を円形ゲージで表示する UI コンポーネント
    /// </summary>
    public class GravityTimerUI : MonoBehaviour
    {
        [SerializeField]
        private Image fillGauge; // Fill Amountで制御するゲージ

        [SerializeField]
        private Image secondLoopGauge; // 二周目用ゲージ

        private GravityOperationManager _manager;

        private float _secondLoopMaxFill;

        private float _totalGaugeCapacity = 1f;

        private void Start()
        {
            _manager = GravityOperationManager.Instance;
            if (_manager == null)
            {
                Debug.LogError("GravityOperationManager が取得できません。");
                return;
            }

            InitializeSecondLoopCapacity();
            
            // 残り時間割合の変更イベントを購読
            _manager.OnOperationRemainingRatioChanged += OnTimerChanged;
            _manager.OnOperationCountChanged += OnOperationCountChanged;
            
            // 初期状態ではゲージをマックスにする
            ApplyGaugeByRatio(1f);
        }

        private void OnDestroy()
        {
            if (_manager == null)
                return;

            _manager.OnOperationRemainingRatioChanged -= OnTimerChanged;
            _manager.OnOperationCountChanged -= OnOperationCountChanged;
        }

        /// <summary>
        /// タイマーの残り割合が変更されたときの処理
        /// </summary>
        private void OnTimerChanged(VGravBehaviour target, float ratio)
        {
            ApplyGaugeByRatio(ratio);
        }

        /// <summary>
        /// 操作数が変更されたときの処理
        /// </summary>
        private void OnOperationCountChanged(int activeCount, int maxCount)
        {
            // 操作中でなければゲージをマックスにする
            if (activeCount == 0)
            {
                ApplyGaugeByRatio(1f);
            }
        }

        private void InitializeSecondLoopCapacity()
        {
            _secondLoopMaxFill = 0f;
            _totalGaugeCapacity = 1f;

            var playerDataController = PlayerDataController.Instance;
            if (playerDataController == null)
                return;

            var upgradeData = playerDataController.GetUpgradeData(UpgradeType.OperationDuration) as ParamUpgrade;
            if (upgradeData == null)
                return;

            var currentLevel = playerDataController.PlayerData.GetLevel(UpgradeType.OperationDuration);
            var maxLevel = upgradeData.UpgradedParams.Length;
            if (maxLevel <= 0)
                return;

            _secondLoopMaxFill = Mathf.Clamp01((float)currentLevel / maxLevel);
            _totalGaugeCapacity = 1f + _secondLoopMaxFill;
        }

        private void ApplyGaugeByRatio(float ratio)
        {
            var clampedRatio = Mathf.Clamp01(ratio);

            if (fillGauge == null)
                return;

            if (secondLoopGauge == null)
            {
                fillGauge.fillAmount = clampedRatio;
                return;
            }

            var totalRemaining = clampedRatio * _totalGaugeCapacity;

            // 二周目が先に減り、二周目が尽きたら一周目を減らす
            var secondLoopFill = Mathf.Clamp(totalRemaining - 1f, 0f, _secondLoopMaxFill);
            var firstLoopFill = Mathf.Clamp01(Mathf.Min(totalRemaining, 1f));

            secondLoopGauge.fillAmount = secondLoopFill;
            secondLoopGauge.gameObject.SetActive(_secondLoopMaxFill > 0f);
            fillGauge.fillAmount = firstLoopFill;
        }
    }
}
