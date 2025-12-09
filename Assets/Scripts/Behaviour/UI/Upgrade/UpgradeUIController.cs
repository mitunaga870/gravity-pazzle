#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.Upgrade
{
    public class UpgradeUIController
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
        }

        #endregion

        #region Handler Methods

        private void HandlerUpgradeOperationDuration()
        {
        }

        private void HandlerUpgradeMaxOperations()
        {
        }

        #endregion
    }
}