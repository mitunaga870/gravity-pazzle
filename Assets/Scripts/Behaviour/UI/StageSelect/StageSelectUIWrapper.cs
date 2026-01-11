#region

using System;
using System.Collections.Generic;
using System.Linq;
using Behaviour.Controller.General.DontDestoroy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.StageSelect
{
    /// <summary>
    ///     stageセレクトUIの配置や表示を管理するラッパークラス
    /// </summary>
    public class StageSelectUIWrapper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private GameObject stageSelectButtonPrefab; // ステージセレクトUIのプレハブ

        [SerializeField]
        private Transform buttonContainer; // ステージセレクトUIの配置先コンテ

        [SerializeField]
        private TMP_Text hoverStageNameText; // ホバー中のステージ名を表示するテキスト

        [SerializeField]
        private TMP_Text hoverStageTitleText; // ホバー中のステージ説明を表示

        [SerializeField]
        private Image hoverStageThumbnailImage; // ホバー中のステージサムネイル画像

        [SerializeField]
        private GameObject nextButton;
        
        [SerializeField]
        private GameObject previousButton;

        #endregion

        private int _curPage;

        private readonly List<GameObject> _stageBtnCache = new();

        public readonly string _defaultStageName = "ステージを選択";

        public readonly string _defaultStageTitle = "どこへ配達しよう";

        private void Start()
        {
            CreateStageCard();
            
            // ボタンにイベントを付ける
            var nextButtonComponent = nextButton.GetComponent<Button>();
            if (nextButtonComponent != null) nextButtonComponent.onClick.AddListener(GoNext);
            var prevButtonComponent = previousButton.GetComponent<Button>();
            if (prevButtonComponent != null) prevButtonComponent.onClick.AddListener(GoPrevious);
            
            // デフォルトテキスト適用
            hoverStageTitleText.text = _defaultStageTitle;
            hoverStageNameText.text = _defaultStageName;
        }

        /// <summary>
        /// ステージセレクトボタンを接地する
        /// </summary>
        /// <param name="pageNum"></param>
        private void CreateStageCard(int pageNum = 0)
        {
            const int pageSize = 4;
            
            // 既存ボタンを破棄
            _stageBtnCache.ForEach((Destroy));
            
            // stageを取得し、ステージセレクトUIを生成・配置する
            var environmentSetting = SettingDataController.Instance.EnvironmentSetting;
            var stages = environmentSetting.Stages;
            
            // 最大4つ要素を抜き出す
            var offset = pageNum * pageSize;
            var showList = stages
                .Skip(offset)
                .Take(pageSize)
                .ToList();

            // ボタンをつくる
            for (var i = 0; i < showList.Count; i++)
            {
                var stage = showList[i];

                var stageSelectButton = Instantiate(stageSelectButtonPrefab, buttonContainer);
                var child = stageSelectButton.transform.GetChild(0);
                
                var button = child.GetComponent<StageSelectButton>();
                if (button == null) throw new Exception("Stage Select Button のプレファブにStage Select Buttonがありません");
                button.Initialize(
                    stage,
                    i + 1,
                    hoverStageNameText,
                    hoverStageTitleText,
                    hoverStageThumbnailImage,
                    _defaultStageName,
                    _defaultStageTitle
                );
                
                // 子要素を取得しずらす
                switch (i)
                {
                    case 1:
                        child.position -= new Vector3(37, 0, 0);
                        break;
                    case 2:
                        child.position -= new Vector3(57, 0, 0);
                        break;
                    case 3:
                        child.position -= new Vector3(29, 0, 0);
                        break;
                }
                    
                _stageBtnCache.Add(stageSelectButton.gameObject);
            }
            
            // 次・前があるか確認
            var hasNext = stages.Count > offset + pageSize;
            nextButton.SetActive(hasNext);
            var hasPrev = pageNum > 0;
            previousButton.SetActive(hasPrev);
            
            _curPage = pageNum;
        }

        private void GoNext()
        {
            CreateStageCard(_curPage + 1);
        }

        private void GoPrevious()
        {
            CreateStageCard(_curPage - 1);
        }
    }
}