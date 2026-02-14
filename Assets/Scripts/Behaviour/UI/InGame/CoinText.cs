using System;
using Behaviour.Controller.General.DontDestoroy;
using TMPro;
using UnityEngine;

namespace Behaviour.UI.InGame
{
    /// <summary>
    /// プレイヤーのコイン数を表示するUI
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class CoinText: MonoBehaviour
    {
        private TMP_Text _text;
        private PlayerDataController  _playerDataController;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            _playerDataController = PlayerDataController.Instance;
        }

        private void Update()
        {
            var coin = _playerDataController.PlayerData.CollectedCoinCount;
            _text.text = coin.ToString();
        }
    }
}