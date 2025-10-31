#region

using System;
using System.Collections;
using UnityEngine;

#endregion

namespace Lib.Logic
{
    public class GeneralUtils
    {
        /// <summary>
        /// 指定時間後にアクションを実行するコルーチンを返す
        /// </summary>
        /// <param name="delay">遅延時間</param>
        /// <param name="action">実行するアクション</param>
        public static IEnumerator DelayCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        
        /// <summary>
        /// ベクトルの最大方向の単位ベクトルを取得する
        /// </summary>
        public static Vector3 GetMaxDirection(Vector3 vector)
        {
            if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y) && Mathf.Abs(vector.x) >= Mathf.Abs(vector.z))
            {
                return new Vector3(Mathf.Sign(vector.x), 0, 0);
            }
            else if (Mathf.Abs(vector.y) >= Mathf.Abs(vector.x) && Mathf.Abs(vector.y) >= Mathf.Abs(vector.z))
            {
                return new Vector3(0, Mathf.Sign(vector.y), 0);
            }
            else
            {
                return new Vector3(0, 0, Mathf.Sign(vector.z));
            }
        }

        /// <summary>
        ///     TimeSpanを"mm:ss"形式の文字列に変換する
        /// </summary>
        public static string TimeSpanToMinuteSecondString(TimeSpan timeSpan)
        {
            var minutes = (int)timeSpan.TotalMinutes;
            var seconds = timeSpan.Seconds;

            return $"{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        ///     TimeSpanからミリ秒部分を取得する
        /// </summary>
        public static int TimeSpanToMilliSec(TimeSpan timeSpan)
        {
            return timeSpan.Milliseconds / 10; // コンマ秒を取得
        }
    }
}