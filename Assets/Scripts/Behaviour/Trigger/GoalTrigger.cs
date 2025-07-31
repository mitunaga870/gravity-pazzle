#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Behaviour.Trigger
{
    public class GoalTrigger : MonoBehaviour
    {
        // ゴールに淘汰すしたときのコールバック処理
        private readonly List<Action> onGoal = new();

        // ゴール時に表示するテキストオブジェクト
        [SerializeField]
        private GameObject goalText;

        /// <summary>
        ///     ゴールに到達したときの処理
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            // プレイヤー以外のオブジェクトがトリガーに入った場合は何もしない
            if (!other.CompareTag("Player")) return;

            // ゴールに到達した場合の処理
            Debug.Log("Goal Reached!");

            // ここでゲームクリアの処理を追加することができます
            goalText.SetActive(true);

            // ゴールに到達したときのコールバックを実行
            foreach (var action in onGloal)
                action?.Invoke();
        }

        /**
         * ゴールに到達したときのコールバックを登録する
         */
        public void AddOnGoal(Action action)
        {
            if (action == null) return;
            
            onGloal.Add(action);
        }
    }
}