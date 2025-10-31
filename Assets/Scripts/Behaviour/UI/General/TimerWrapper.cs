#region

using System;
using Behaviour.Controller.Stage;
using Behaviour.Trigger;
using Lib.Logic;
using TMPro;
using UnityEngine;

#endregion

namespace Behaviour.UI.General
{
    /// <summary>
    ///     時間をカウントアップしてTMPに反映するコンポーネント
    /// </summary>
    public class TimerWrapper : MonoBehaviour
    {
        // 分：秒を表示するためのTMPコンポーネント
        [SerializeField]
        private TMP_Text mainText;

        // コンマ秒を表示するためのTMPコンポーネント
        [SerializeField]
        private TMP_Text subText;

        // タイマーの現在の時間
        private TimeSpan currentTime;

        // 停止フラグ
        private bool isStopped;

        private StageDataController _stageDataController;

        private void Start()
        {
            // 初期化
            currentTime = TimeSpan.Zero;
            _stageDataController = StageDataController.Instance;

            // クリア時にとめる
            // ゴールトリガーを取得
            var goals = FindObjectsByType<GoalTrigger>(FindObjectsSortMode.None);
            foreach (var goal in goals)
                // ゴールに到達したときのコールバックを登録
                goal.AddOnGoal(() =>
                {
                    isStopped = true; // タイマーを停止
                });
        }

        private void Update()
        {
            if (isStopped)
                return; // 停止中は何もしない
            
            // タイマーの更新
            currentTime += TimeSpan.FromSeconds(Time.deltaTime);
            _stageDataController.PlayTime = currentTime;

            // 表示の更新
            // 分と秒を計算
            var milliseconds = GeneralUtils.TimeSpanToMilliSec(currentTime); // 2桁のコンマ秒

            // テキストの更新
            mainText.text = GeneralUtils.TimeSpanToMinuteSecondString(currentTime);
            subText.text = $"{milliseconds:D2}";
            
        }
    }
}