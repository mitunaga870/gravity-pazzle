#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using Behaviour.Trigger;
using Lib.Logic;
using ScriptableObj.Setting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI.InGame
{
    public class ClearUIWrapper : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text stageNameText;

        [SerializeField]
        private TMP_Text stageTitleText;
        
        [SerializeField]
        private TMP_Text clearTimeTextMain;

        [SerializeField]
        private TMP_Text clearTimeTextSub;

        [SerializeField]
        private Image[] starImages;

        [SerializeField]
        private Sprite starOnSprite;

        [SerializeField]
        private Sprite starOffSprite;

        private StageDataController StageDataController => StageDataController.Instance;

        private StageSetting _stageSetting;

        // 星が一つ減る時間の閾値（秒）
        private const int StartThresholdSec = 30;

        private void Start()
        {
            var goalTriggers = FindObjectsByType<GoalTrigger>(FindObjectsSortMode.None);
            foreach (var goalTrigger in goalTriggers) goalTrigger.AddOnGoal(OnGoal);

            int stageNum;
            (_stageSetting, stageNum) = SettingDataController.Instance.EnvironmentSetting.GetFromCurScene();

            stageNameText.text = $"ステージ{stageNum + 1}";
            stageTitleText.text = _stageSetting.DisplayName;

            gameObject.SetActive(false);
        }

        private void OnGoal()
        {
            // クリアタイム表示の更新
            var playTime = StageDataController.PlayTime;
            clearTimeTextMain.text = GeneralUtils.TimeSpanToMinuteSecondString(playTime);
            clearTimeTextSub.text = ":" + GeneralUtils.TimeSpanToCentiSec(playTime).ToString("D2");

            // 星の数の計算
            var clearTimeSec = (int)playTime.TotalSeconds;
            var estimatedClearTimeSec = _stageSetting.ClearTimeMinutes * 60;
            var diffSec = clearTimeSec - estimatedClearTimeSec;
            var decreaseStars =
                diffSec <= 0 ? 0 : Mathf.CeilToInt((float)diffSec / StartThresholdSec);
            var stars = Math.Max(0, 3 - decreaseStars);

            // 星の表示更新
            for (var i = 1; i <= 3; i++)
                starImages[i - 1].sprite = i <= stars ? starOnSprite : starOffSprite;

            gameObject.SetActive(true);
        }
    }
}