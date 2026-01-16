using Behaviour.Controller.General.DontDestoroy;
using Lib.DataClass.PlayData;
using Lib.Logic.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviour.UI.Upgrade
{
    /// <summary>
    /// テストようにコイン数の表示及びコインの獲得を行う
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class TestCoinButton : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        
        private PlayerDataController _playerDataController;
        
        private void Start()
        {
            _playerDataController = PlayerDataController.Instance;
            
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void Update()
        {
            text.text = $"現在コイン所持数（クリックで増加）：{_playerDataController.PlayerData.CollectedCoinCount}";
        }

        private void OnClick()
        {
            _playerDataController.CollectCoin(1);
        }
    }
}