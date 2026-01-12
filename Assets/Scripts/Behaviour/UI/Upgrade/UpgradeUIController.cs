#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using ScriptableObj.Upgrade;
using TMPro;
using UnityEngine;

#endregion

namespace Behaviour.UI.Upgrade
{
    public class UpgradeUIController : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("UI要素：概要")]
        [SerializeField]
        private TMP_Text hovTitle;
        
        [SerializeField]
        private TMP_Text hovDescription;

        [SerializeField]
        private TMP_Text hovContent;
        
        [SerializeField]
        private TMP_Text hovCost;

        [Header("UI要素：各強化要素の強化ボタン")]
        [SerializeField]
        private UpgradeUIButton upgradeOperationDurationButton;

        [SerializeField]
        private UpgradeUIButton upgradeMaxOperationsButton;

        [SerializeField]
        private UpgradeUIButton enablePlayerGravChangeButton;

        #endregion

        #region Private Fields

        private PlayerDataController _playerDataController;

        #endregion

        #region Unity Methods

        private void Start()
        {
            // PlayerDataController取得
            _playerDataController = PlayerDataController.Instance;
            if (_playerDataController == null) throw new Exception("PlayerDataController is not assigned.");

            // ハンドラー登録
            var durationUpgradeData = GetUpgradeData(UpgradeType.OperationDuration);
            upgradeOperationDurationButton.Init(
                HandlerUpgradeOperationDuration,
                durationUpgradeData.curLevel,
                durationUpgradeData.title,
                hovTitle,
                durationUpgradeData.description,
                hovDescription,
                durationUpgradeData.content,
                hovContent,
                durationUpgradeData.cost,
                hovCost
                );

            var maxOperationsUpgradeData = GetUpgradeData(UpgradeType.MaxOperationCount);
            upgradeMaxOperationsButton.Init(
                HandlerUpgradeMaxOperations,
                maxOperationsUpgradeData.curLevel,
                maxOperationsUpgradeData.title,
                hovTitle,
                maxOperationsUpgradeData.description,
                hovDescription,
                maxOperationsUpgradeData.content,
                hovContent,
                maxOperationsUpgradeData.cost,
                hovCost
            );

            var gravChangeUpgradeData = GetUpgradeData(UpgradeType.PlayerGravChange);
            enablePlayerGravChangeButton.Init(
                HandlerPlayerGravChange,
                gravChangeUpgradeData.curLevel,
                gravChangeUpgradeData.title,
                hovTitle,
                gravChangeUpgradeData.description,
                hovDescription,
                gravChangeUpgradeData.content,
                hovContent,
                gravChangeUpgradeData.cost,
                hovCost
            );

            CheckUpgradeable();
        }

        #endregion

        #region Handler Methods

        private void HandlerUpgradeOperationDuration()
        {
            _playerDataController.Upgrade(UpgradeType.OperationDuration);
            CheckUpgradeable();
        }

        private void HandlerUpgradeMaxOperations()
        {
            _playerDataController.Upgrade(UpgradeType.MaxOperationCount);
            CheckUpgradeable();
        }

        private void HandlerPlayerGravChange()
        {
            _playerDataController.Upgrade(UpgradeType.PlayerGravChange);
            CheckUpgradeable();
        }

        #endregion

        private (int curLevel, string title, string description, string content, int cost) GetUpgradeData(UpgradeType type)
        {
            var playerData = _playerDataController.PlayerData;
            var curLevel = playerData.GetLevel(type);

            var upgradeData = _playerDataController.GetUpgradeData(type);
            var title = upgradeData.DisplayName;
            var description = upgradeData.Description;
            var cost = upgradeData.Cost[curLevel];

            string content;
            switch (upgradeData)
            {
                case ParamUpgrade paramUpgrade:
                {
                    var unit = paramUpgrade.Unit;
                    var param = paramUpgrade.UpgradedParams;

                    var cur = param[curLevel];
                    var next = param[curLevel + 1];
                    var diff = next -cur;

                    content = $"{cur}{unit} →　{next}{unit}(↑{diff})";
                    break;
                }
                case ActionUpgrade actionUpgrade:
                    content = actionUpgrade.Content;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return (curLevel, title, description, content, cost);
        }

        private void SetActiveButton(UpgradeType type, bool enable)
        {
            switch (type)
            {
                case UpgradeType.OperationDuration:
                    upgradeOperationDurationButton.gameObject.SetActive(enable);
                    break;
                case UpgradeType.MaxOperationCount:
                    upgradeMaxOperationsButton.gameObject.SetActive(enable);
                    break;
                case UpgradeType.PlayerGravChange:
                    enablePlayerGravChangeButton.gameObject.SetActive(enable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void CheckUpgradeable()
        {
            // 強化可能でない場合の処理
            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                var upgradeable = _playerDataController.IsUpgradeable(type);
                SetActiveButton(type, upgradeable);
            }
        }
    }
}