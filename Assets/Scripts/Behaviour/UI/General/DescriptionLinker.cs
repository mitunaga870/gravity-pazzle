#region

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Behaviour.UI.General
{
    /// <summary>
    ///     概要をテキストに表示するためのクラス
    /// </summary>
    public class DescriptionLinker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private string description;

        [SerializeField]
        private TMP_Text descriptionText;

        public void OnPointerEnter(PointerEventData eventData)
        {
            descriptionText.text = description;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            descriptionText.text = string.Empty;
        }

        /// <summary>
        ///     表示対象オブジェクトと内容を設定
        /// </summary>
        public void Setup(string descriptionArg, TMP_Text descriptionTextArg)
        {
            description = descriptionArg;
            descriptionText = descriptionTextArg;
        }
    }
}