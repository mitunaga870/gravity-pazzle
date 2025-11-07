#region

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
        private StageSelectButton stageSelectButtonPrefab; // ステージセレクトUIのプレハブ

        [SerializeField]
        private Transform buttonContainer; // ステージセレクトUIの配置先コンテ

        [SerializeField]
        private TMP_Text hoverStageNameText; // ホバー中のステージ名を表示するテキスト

        [SerializeField]
        private TMP_Text hoverStageTitleText; // ホバー中のステージ説明を表示

        [SerializeField]
        private Image hoverStageThumbnailImage; // ホバー中のステージサムネイル画像

        #endregion


        private void Start()
        {
            // stageを取得し、ステージセレクトUIを生成・配置する
            var environmentSetting = SettingDataController.Instance.EnvironmentSetting;
            var stages = environmentSetting.Stages;

            for (var i = 0; i < stages.Count; i++)
            {
                var stage = stages[i];

                var stageSelectButton = Instantiate(stageSelectButtonPrefab, buttonContainer);
                stageSelectButton.Initialize(stage, i + 1, hoverStageNameText, hoverStageTitleText,
                    hoverStageThumbnailImage);
            }
        }
    }
}