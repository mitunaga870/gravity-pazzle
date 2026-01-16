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
    [RequireComponent(typeof(Image))]
    public class UpgradeUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        # region Serialize Field
        [SerializeField]
        private TMP_Text curLevelText;
        
        [SerializeField]
        private TMP_Text nextLevelText;

        [SerializeField]
        private TMP_Text costText;
        
        [SerializeField]
        private TMP_Text costSubText;
        
        [SerializeField]
        private Sprite defaultSprite;

        [SerializeField]
        private Sprite hovSprite;
        
        [SerializeField]
        private Color defaultMainColor = Color.white;
        
        [SerializeField]
        private Color defaultAccentColor = new (230, 29, 79);
        
        [SerializeField]
        private Color hovMainColor = new (46, 46, 46);
        
        [SerializeField]
        private Color hovAccentColor = Color.white;
        
        # endregion 
        
        # region Private Field
        
        private Image _background;

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

        private void Start()
        {
            _background = GetComponent<Image>();
        }

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
            // 強化イベント実装
            var btn = gameObject.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(onclick);
            
            // ボタンのテキスト設定
            curLevelText.text = curLevel.ToString("D2");
            nextLevelText.text = (curLevel + 1).ToString("D2");
            costText.text = cost.ToString("D2");
            
            // ホバー時のテキスト設定
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
            // テキスト設定
            _titleText.text = _title;
            _descriptionText.text = _description;
            _contentText.text = _content;
            _hovCostText.text = _cost;
            
            // 背景変更・文字色変更
            _background.sprite = hovSprite;
            curLevelText.color = hovMainColor;
            nextLevelText.color = hovAccentColor;
            costText.color = hovMainColor;
            costSubText.color = hovMainColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // テキスト設定
            _titleText.text = DefaultTitle;
            _descriptionText.text = DefaultDescription;
            _contentText.text = string.Empty;
            _hovCostText.text = string.Empty;
            
            // 背景変更・文字色変更
            _background.sprite = defaultSprite;
            curLevelText.color = defaultMainColor;
            nextLevelText.color = defaultAccentColor;
            costText.color = defaultMainColor;
            costSubText.color = defaultMainColor;
        }
    }
}