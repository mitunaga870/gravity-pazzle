#region

using System;
using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
using Lib.State.Scene;
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

        private SceneStateController _sceneStateController;

        private InputController _inputController;

        #endregion

        #region Unity Methods

        private void Start()
        {
            // PlayerDataController取得
            _playerDataController = PlayerDataController.Instance;
            if (_playerDataController == null) throw new Exception("PlayerDataController is not assigned.");

            // SceneStateController取得
            _sceneStateController = SceneStateController.Instance;
            if (_sceneStateController == null) throw new Exception("SceneStateController is not assigned.");

            // InputController取得
            _inputController = InputController.Instance;
            if (_inputController == null) throw new Exception("InputController is not assigned.");

            // ハンドラー登録
            upgradeOperationDurationButton.onClick.AddListener(HandlerUpgradeOperationDuration);
            upgradeMaxOperationsButton.onClick.AddListener(HandlerUpgradeMaxOperations);
            enablePlayerGravChangeButton.onClick.AddListener(HandlerPlayerGravChange);

            CheckUpgradeable();
        }

        private void Update()
        {
            if (_inputController.GetKey(KeyCode.Escape, SceneState.Upgrade)) HideUpgradeUI();
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

        public void ShowUpgradeUI()
        {
            gameObject.SetActive(true);
            _sceneStateController.ChangeSceneState(SceneState.Upgrade);
        }

        private void HideUpgradeUI()
        {
            _sceneStateController.ReturnPrevSceneState();
            gameObject.SetActive(false);
        }
    }
}