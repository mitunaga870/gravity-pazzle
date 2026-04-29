#region

using System.Collections.Generic;
using Coffee.UIEffects;
using ScriptableObj;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Behaviour.Controller.General.DontDestoroy;

#endregion

namespace Behaviour.UI.StageSelect
{
    /// <summary>
    ///     マウスホバー時のエフェクトなど、ステージセレクトボタンに関するUIエフェクトを管理するクラス
    /// </summary>
    [RequireComponent(typeof(UIEffect))]
    [RequireComponent(typeof(UIEffectTweener))]
    public class StageSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        [SerializeField]
        private TMP_Text stageId;

        [SerializeField]
        private TMP_Text stageTitle;

        [SerializeField]
        private GameObject selectedIcon;
        
        [Header("ステージ名の文字色")]
        [SerializeField]
        private Color defaultTitleColor = Color.white;
        
        [SerializeField]
        private Color hovTitleColor = Color.black;
        
        [Header("スプライト")]
        [SerializeField]
        private Sprite defaultSprite;
        
        [SerializeField]
        private Sprite hovSprite;

        private Image _backGround;
        private Image _stageThumbnail;

        // エフェクト関連
        private UIEffect _uiEffect;
        private readonly List<UIEffectTweener> _uiEffectTweeners = new();
        private readonly List<UIEffect> _uiEffects = new();

        // ホバーじのステージ名表示用テキスト
        private TMP_Text _hoverStageNameText;
        private TMP_Text _hoverStageTitleText;

        // 渡された情報
        private StageData _stage;
        private int _stageNumber;
        
        // ホバー解除時のテキスト
        private string _defaultStageName;
        private string _defaultStageTitle;

        private void Awake()
        {
            // 自身のコンポーネントを取得
            _uiEffect = GetComponent<UIEffect>();
            var uiEffectTweener = GetComponent<UIEffectTweener>();

            _uiEffect.enabled = false;
            _uiEffectTweeners.Add(uiEffectTweener);

            // 子オブジェクトのUIEffectTweenerコンポーネントも取得
            var childTweeners = GetComponentsInChildren<UIEffectTweener>();
            foreach (var tweener in childTweeners)
                if (tweener != uiEffectTweener)
                    _uiEffectTweeners.Add(tweener);

            // 子オブジェクトのUIEffectコンポーネントも取得
            var childEffects = GetComponentsInChildren<UIEffect>();
            foreach (var effect in childEffects)
                if (effect != _uiEffect)
                    _uiEffects.Add(effect);
            
            // 見た目変更
            _backGround = GetComponent<Image>();
        }

        #region Pointer Event Handlers

        /// <summary>
        ///     ホバー開始時の処理
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // ホバー時のエフェクトを有効化
            _uiEffect.enabled = true;

            // サムネイル画像を設定
            _stageThumbnail.sprite = _stage.StageThumbnail;
            _stageThumbnail.gameObject.SetActive(true);

            // ホバー中のステージ名・説明文を表示
            _hoverStageNameText.text = _stage.DisplayName;
            _hoverStageTitleText.text = $"ステージ {_stageNumber}";
            
            // 見た目変更
            _backGround.sprite = hovSprite;
            stageTitle.color = hovTitleColor;
            selectedIcon.SetActive(true);
        }

        /// <summary>
        ///     ホバー終了時の処理
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // ホバー時のエフェクトを無効化
            _uiEffect.enabled = false;

            // サムネイル画像をクリア
            _stageThumbnail.gameObject.SetActive(false);

            // ホバー中のステージ名・説明文をクリア
            _hoverStageNameText.text = _defaultStageName;
            _hoverStageTitleText.text = _defaultStageName;
            
            // 見た目変更
            _backGround.sprite = defaultSprite;
            stageTitle.color = defaultTitleColor;
            selectedIcon.SetActive(false);
        }

        /// <summary>
        ///     クリック時の処理
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // クリック時のエフェクトを再生
            foreach (var effect in _uiEffects)
                effect.enabled = true;
            foreach (var tweener in _uiEffectTweeners)
                tweener.PlayForward();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     stageを指定してステージセレクトボタンを初期化する
        /// </summary>
        public void Initialize(
            StageData stage,
            int stageNumber,
            TMP_Text hoverStageNameText,
            TMP_Text hoverStageTitleText,
            Image thumbnailImage,
            string defaultStageName,
            string defaultStageTitle
        )
        {
            stageId.text = $"ステージ {stageNumber}";
            stageTitle.text = stage.DisplayName;
            _stage = stage;
            _stageNumber = stageNumber;
            _hoverStageNameText = hoverStageNameText;
            _hoverStageTitleText = hoverStageTitleText;
            _stageThumbnail = thumbnailImage;
            _defaultStageName = defaultStageName;
            _defaultStageTitle = defaultStageTitle;
        }

        /// <summary>
        ///     ステージ遷移
        /// </summary>
        public void TransitionToStage()
        {
            var soundController = SoundController.Instance;
            if (soundController != null)
            {
                soundController.PlaySe("SceneTransition");
            }

            SceneManager.LoadScene(_stage.StageScene);
        }

        #endregion
    }
}