#region

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Behaviour.UI.General
{
    /// <summary>
    ///     設定したUIをクリックしたら指定したURLに飛ぶボタン
    /// </summary>
    public class ButtonToLinkURL : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private TMP_Text urlText;

        public void OnPointerClick(PointerEventData eventData)
        {
            // URLを取得してブラウザで開く
            var linkInfos = urlText.textInfo.linkInfo;
            if (linkInfos.Length <= 0) return;
            var url = linkInfos[0].GetLinkID();
            Application.OpenURL(url);
        }
    }
}