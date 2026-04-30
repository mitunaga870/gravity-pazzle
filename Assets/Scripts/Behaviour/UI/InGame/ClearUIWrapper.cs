#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using Behaviour.Trigger;
using Lib.Logic;
using ScriptableObj;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private StageData _stageData;

        // 星が一つ減る時間の閾値（秒）
        private const int StartThresholdSec = 30;

        private bool _movableToNext;

        private void Start()
        {
            var goalTriggers = FindObjectsByType<GoalTrigger>(FindObjectsSortMode.None);
            foreach (var goalTrigger in goalTriggers) goalTrigger.AddOnGoal(OnGoal);

            int stageNum;
            (_stageData, stageNum) = SettingDataController.Instance.EnvironmentSetting.GetFromCurScene();

            stageNameText.text = $"ステージ{stageNum + 1}";
            stageTitleText.text = _stageData.DisplayName;

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_movableToNext && Input.anyKeyDown)
            {
                StartCoroutine(ReturnToStageSelectWithSceneTransitionSe());
            }
        }

        private void OnGoal()
        {
            // クリアタイム表示の更新
            var playTime = StageDataController.PlayTime;
            clearTimeTextMain.text = GeneralUtils.TimeSpanToMinuteSecondString(playTime);
            clearTimeTextSub.text = ":" + GeneralUtils.TimeSpanToCentiSec(playTime).ToString("D2");

            // 星の数の計算
            var clearTimeSec = (int)playTime.TotalSeconds;
            var estimatedClearTimeSec = _stageData.ClearTimeMinutes * 60;
            var diffSec = clearTimeSec - estimatedClearTimeSec;
            var decreaseStars =
                diffSec <= 0 ? 0 : Mathf.CeilToInt((float)diffSec / StartThresholdSec);
            var stars = Math.Max(0, 3 - decreaseStars);

            // 星の表示更新
            for (var i = 1; i <= 3; i++)
                starImages[i - 1].sprite = i <= stars ? starOnSprite : starOffSprite;

            // マウスロックを解除
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 1秒後に進行可能にして表示
            gameObject.SetActive(true);
            var delay = GeneralUtils.DelayCoroutine(1f, () => { _movableToNext = true; });
            StartCoroutine(delay);
        }

        private System.Collections.IEnumerator ReturnToStageSelectWithSceneTransitionSe()
        {
            var soundController = SoundController.Instance;
            if (soundController != null)
            {
                soundController.PlaySe("SceneTransition");
            }

            yield return new WaitForSeconds(SoundController.GetSceneTransitionDelaySeconds());

            SceneManager.LoadScene(SettingDataController.Instance.EnvironmentSetting.StageSelectScene);
        }
    }
}