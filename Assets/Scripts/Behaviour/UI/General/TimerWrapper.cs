#region

using System;
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

        private void Start()
        {
            // 初期化
            currentTime = TimeSpan.Zero;
        }

        private void Update()
        {
            // タイマーの更新
            currentTime += TimeSpan.FromSeconds(Time.deltaTime);

            // 表示の更新
            // 分と秒を計算
            var minutes = (int)currentTime.TotalMinutes;
            var seconds = currentTime.Seconds;
            var milliseconds = currentTime.Milliseconds / 10; // 2桁のコンマ秒

            // テキストの更新
            mainText.text = $"{minutes:D2}:{seconds:D2}";
            subText.text = $"{milliseconds:D2}";
        }
    }
}