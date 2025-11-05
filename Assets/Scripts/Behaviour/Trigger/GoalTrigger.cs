#region

using System;
using System.Collections.Generic;
using Behaviour.Gravity.Abstract;
using Behaviour.Player.Abstract;
using UnityEngine;

#endregion

namespace Behaviour.Trigger
{
    public class GoalTrigger : MonoBehaviour
    {
        // ゴールに到達したときのコールバック処理
        private readonly List<Action<APlayerBehaviour, AGravBehaviour>> _onGoal = new();
        
        // ゴール時に表示するテキストオブジェクト
        [SerializeField]
        private GameObject goalText;

#if UNITY_EDITOR
        private void Update()
        {
            // デバッグ用: Gキーでゴール処理を強制的に呼び出す
            if (Input.GetKeyDown(KeyCode.G)) OnTriggerEnter(GameObject.FindWithTag("Player").GetComponent<Collider>());
        }
#endif


        /// <summary>
        ///     ゴールに到達したときの処理
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            // プレイヤー以外のオブジェクトがトリガーに入った場合は何もしない
            if (!other.CompareTag("Player")) return;

            // ゴールに到達したときのコールバックを実行
            var playerBehaviour = other.GetComponent<APlayerBehaviour>();
            if (playerBehaviour == null) throw new Exception("PlayerBehaviour component not found on Player object.");

            var gravBehaviour = other.GetComponent<AGravBehaviour>();
            if (gravBehaviour == null) throw new Exception("GravBehaviour component not found on Player object.");

            foreach (var action in _onGoal)
                action?.Invoke(playerBehaviour, gravBehaviour);
        }
        
        /// <summary>
        ///     ゴールに到達したときのコールバックを登録する
        /// </summary>
        public void AddOnGoal(Action action)
        {
            if (action == null) return;

            // ラップしたアクションを登録
            void WrappedAction(APlayerBehaviour _, AGravBehaviour __)
            {
                action();
            }

            _onGoal.Add(WrappedAction);
        }

        /// <summary>
        ///     ゴールに到達したときのコールバックを登録する
        /// </summary>
        public void AddOnGoal(Action<APlayerBehaviour, AGravBehaviour> action)
        {
            if (action == null) return;

            _onGoal.Add(action);
        }
    }
}