using System.Collections;
using UnityEngine;

namespace Lib.Logic
{
    public class GeneralUtils
    {
        /// <summary>
        /// 指定時間後にアクションを実行するコルーチンを返す
        /// </summary>
        /// <param name="delay">遅延時間</param>
        /// <param name="action">実行するアクション</param>
        public static IEnumerator DelayCoroutine(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}