#region

using System;
using System.Diagnostics;
using Behaviour.Controller.General.DontDestoroy;
using LitMotion;
using LitMotion.Extensions;
using ScriptableObj.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
            
            InitButton(UpgradeType.OperationDuration);
            InitButton(UpgradeType.MaxOperationCount);
            InitButton(UpgradeType.PlayerGravChange);

            CheckUpgradeable();
        }

        #endregion

        #region Handler Methods

        private void HandlerUpgradeOperationDuration()
        {
            HandlerUpgradeExe(UpgradeType.OperationDuration);
        }

        private void HandlerUpgradeMaxOperations()
        {
            HandlerUpgradeExe(UpgradeType.MaxOperationCount);
        }

        private void HandlerPlayerGravChange()
        {
            HandlerUpgradeExe(UpgradeType.PlayerGravChange);
        }

        private void HandlerUpgradeExe(UpgradeType type)
        {
            var result = _playerDataController.Upgrade(type);
            
            if (result)
            {
                // 強化成功時の初期化
                InitButton(UpgradeType.PlayerGravChange);
                CheckUpgradeable();
            }
            else
            {
                // 失敗時は揺らす
                var button = GetButton(type);
                var rectTransform = button.GetComponent<RectTransform>();
                LMotion.Shake.Create(rectTransform.position, new Vector3(5, 0), 0.5f)
                    .BindToPosition(rectTransform)
                    .AddTo(button);
            }
        }

        #endregion

        private UpgradeUIButton GetButton(UpgradeType type)
        {
            return type switch
            {
                UpgradeType.OperationDuration => upgradeOperationDurationButton,
                UpgradeType.MaxOperationCount => upgradeMaxOperationsButton,
                UpgradeType.PlayerGravChange => enablePlayerGravChangeButton,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

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
            // ボタン初期化
            if (enable) InitButton(type);

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

        private void InitButton(UpgradeType type)
        {
            // アップグレード可能か判別
            var upgradeable = _playerDataController.IsUpgradeable(type);
            if(!upgradeable) return;

            // 情報取得
            var upgradeData = GetUpgradeData(type);
            var button = GetButton(type);
            UnityAction onClick = type switch
            {
                UpgradeType.OperationDuration => HandlerUpgradeOperationDuration,
                UpgradeType.MaxOperationCount => HandlerUpgradeMaxOperations,
                UpgradeType.PlayerGravChange => HandlerPlayerGravChange,
                _ => throw new NotImplementedException()
            };
            
            button.Init(
                onClick,
                upgradeData.curLevel,
                upgradeData.title,
                hovTitle,
                upgradeData.description,
                hovDescription,
                upgradeData.content,
                hovContent,
                upgradeData.cost,
                hovCost
                );
        }
    }
}