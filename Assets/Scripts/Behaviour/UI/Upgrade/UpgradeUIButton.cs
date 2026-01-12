using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Behaviour.UI.Upgrade
{
    /// <summary>
    /// アップグレードそれぞれのボタン用UI
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UpgradeUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        # region Serialize Field
        [SerializeField]
        private TMP_Text curLevelText;
        
        [SerializeField]
        private TMP_Text nextLevelText;
        
        [SerializeField]
        private TMP_Text costText;
        
        # endregion 
        
        # region Private Field

        private const string DefaultTitle = "強化項目を選択";
        
        private string _title;
        
        private TMP_Text _titleText;

        private const string DefaultDescription = "何を強化しよう...";
        
        private string _description;
        
        private TMP_Text _descriptionText;

        private string _content;
        
        private TMP_Text _contentText;
        
        private string _cost;
        
        private TMP_Text _hovCostText;
        
        #endregion

        public void Init(
            UnityAction onclick,
            int curLevel,
            string title,
            TMP_Text titleText,
            string description,
            TMP_Text descriptionText,
            string content,
            TMP_Text contentText, 
            int cost,
            TMP_Text hovCostText)
        {
            var btn = gameObject.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(onclick);
            
            curLevelText.text = curLevel.ToString("D2");
            nextLevelText.text = (curLevel + 1).ToString("D2");
            costText.text = cost.ToString("D2");
            
            _title = title;
            _titleText = titleText;
            _description = description;
            _descriptionText = descriptionText;
            _content = $"効果：{content}";
            _contentText = contentText;
            _cost = $"コスト：{cost}";
            _hovCostText = hovCostText;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _titleText.text = _title;
            _descriptionText.text = _description;
            _contentText.text = _content;
            _hovCostText.text = _cost;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _titleText.text = DefaultTitle;
            _descriptionText.text = DefaultDescription;
            _contentText.text = string.Empty;
            _hovCostText.text = string.Empty;
        }
    }
}