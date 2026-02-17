#region

using System;
using System.Collections.Generic;
using Lib.State.Scene;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Behaviour.Controller.General
{
    public class SceneStateController : MonoBehaviour
    {
        public static SceneStateController Instance { get; private set; }

        /// <summary>
        ///     状態操作コンストラクタ
        /// </summary>
        public SceneStateContext Context { get; protected set; }

        /// <summary>
        ///     初期状態
        /// </summary>
        [SerializeField]
        private SceneState initialState;

        private ISceneState _prev;
        
        private Dictionary<SceneState, List<UnityAction>> _onSceneStateChanged;

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // コンテキストの初期化
            Context = new SceneStateContext(
                SceneStateUtils.GenerateState(initialState));
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     シーン状態を変更する。
        /// </summary>
        /// <param name="next">次の状態</param>
        /// <param name="forceChange">強制的に変更するかどうか</param>
        public void ChangeSceneState(SceneState next, bool forceChange = false)
        {
            Context ??= new SceneStateContext(
                SceneStateUtils.GenerateState(initialState));

            _prev = Context.CurrentState;
            // 状態を変更する
            Context.Change(SceneStateUtils.GenerateState(next), forceChange);
            
            // 状態変更イベントを呼び出す
            if (_onSceneStateChanged != null && _onSceneStateChanged.TryGetValue(next, out var actions))
            {
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }

        /// <summary>
        ///     一個前のシーンに遷移する
        /// </summary>
        public void ReturnPrevSceneState(bool forceChange = false)
        {
            Context ??= new SceneStateContext(SceneStateUtils.GenerateState(initialState));
            Context.Change(_prev, forceChange);
        }
        
        /// <summary>
        /// 特定のシーン状態に遷移したときのイベントを追加する。
        /// </summary>
        /// <param name="sceneState"></param>
        /// <param name="action"></param>
        public void AddOnSceneStateChanged(SceneState sceneState, UnityAction action)
        {
            _onSceneStateChanged ??= new Dictionary<SceneState, List<UnityAction>>();

            if (_onSceneStateChanged.ContainsKey(sceneState))
            {
                _onSceneStateChanged[sceneState].Add(action);
            }
            else
            {
                _onSceneStateChanged.Add(sceneState, new List<UnityAction> { action });
            }
        }

        #endregion
    }
}