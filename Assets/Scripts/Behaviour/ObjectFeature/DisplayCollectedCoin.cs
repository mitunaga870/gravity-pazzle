#region

using Behaviour.Controller.General.DontDestoroy;
using TMPro;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature
{
    /// <summary>
    ///     取得したコインの数をただ表示する
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DisplayCollectedCoin : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;
        private PlayerDataController _playerDataController;

        private void Start()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            _playerDataController = PlayerDataController.Instance;
        }

        private void Update()
        {
            _textMeshProUGUI.text = _playerDataController.PlayerData.CollectedCoinCount.ToString();
        }
    }
}