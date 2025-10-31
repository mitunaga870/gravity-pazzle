#region

using System.Collections.Generic;
using Behaviour.Controller.General.DontDestoroy;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.UI.StageSelect
{
    /// <summary>
    ///     マウスホバー時のエフェクトなど、ステージセレクトボタンに関するUIエフェクトを管理するクラス
    /// </summary>
    [RequireComponent(typeof(UIEffect))]
    [RequireComponent(typeof(UIEffectTweener))]
    public class StageSelectButtonUIEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        [SerializeField]
        [Tooltip("クリック時に移動するステージ番号")]
        private int stageNumber;

        private UIEffect _uiEffect;
        private readonly List<UIEffectTweener> _uiEffectTweeners = new();

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
        }

        #region Pointer Event Handlers

        /// <summary>
        ///     ホバー開始時の処理
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _uiEffect.enabled = true;
        }

        /// <summary>
        ///     ホバー終了時の処理
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            _uiEffect.enabled = false;
        }

        /// <summary>
        ///     クリック時の処理
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            foreach (var tweener in _uiEffectTweeners)
                tweener.PlayForward();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     ステージ遷移
        /// </summary>
        public void TransitionToStage()
        {
            var stage = SettingDataController.Instance.EnvironmentSetting.StageScenes[stageNumber];
            SceneManager.LoadScene(stage);
        }

        #endregion
    }
}