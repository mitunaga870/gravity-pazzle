#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using ScriptableObj.Upgrade;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.Upgrade
{
    public class UpgradeUIController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI要素：各強化要素の強化ボタン")]
        [SerializeField]
        private Button upgradeOperationDurationButton;

        [SerializeField]
        private Button upgradeMaxOperationsButton;

        [SerializeField]
        private Button enablePlayerGravChangeButton;

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
            upgradeOperationDurationButton.onClick.AddListener(HandlerUpgradeOperationDuration);
            upgradeMaxOperationsButton.onClick.AddListener(HandlerUpgradeMaxOperations);
            enablePlayerGravChangeButton.onClick.AddListener(HandlerPlayerGravChange);

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

        private void DisableButton(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.OperationDuration:
                    upgradeOperationDurationButton.gameObject.SetActive(false);
                    break;
                case UpgradeType.MaxOperationCount:
                    upgradeMaxOperationsButton.gameObject.SetActive(false);
                    break;
                case UpgradeType.PlayerGravChange:
                    enablePlayerGravChangeButton.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void CheckUpgradeable()
        {
            // 強化可能出ない場合の処理
            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                var upgradeable = _playerDataController.IsUpgradeable(type);
                if (!upgradeable) DisableButton(type);
            }
        }
    }
}