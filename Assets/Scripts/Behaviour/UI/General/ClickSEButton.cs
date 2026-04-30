using Behaviour.Controller.General.DontDestoroy;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviour.UI.General
{
    /// <summary>
    /// ボタンがクリックされたときに、SoundController 経由で SE を再生するコンポーネント。
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ClickSEButton : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("BgmSeData.SeList に登録されたクリックSEのID")]
        private string seId = "Click";

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(OnClick);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (string.IsNullOrEmpty(seId))
            {
                return;
            }

            var soundController = SoundController.Instance;
            if (soundController == null)
            {
                Debug.LogWarning("ClickSEButton: SoundController.Instance が見つからないため、SE を再生できません。");
                return;
            }

            soundController.PlaySe(seId);
        }
    }
}

