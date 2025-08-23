#region

using System.Collections.Generic;
using Lib.State.Scene;
using UnityEngine;

#endregion

namespace Behaviour.Controller.General
{
    /// <summary>
    ///     ユーザー入力を処理するためのコントローラー
    ///     ゲーム状況とかでキー判定するために処理を集約する。
    ///     状態操作も必須とする
    /// </summary>
    [RequireComponent(typeof(SceneStateController))]
    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private SceneStateController sceneStateController;

        public string InputString => Input.inputString;

        private SceneStateContext StateContext => sceneStateController.Context;

        #region GetKey

        /// <summary>
        ///     キー入力状態を取得する
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <returns>キーが押されているかどうか</returns>
        public bool GetKey(KeyCode key)
        {
            // キー入力をチェック
            return Input.GetKey(key);
        }

        /// <summary>
        ///     キー入力状態を取得する
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <param name="target">入力を許す状態</param>
        /// <returns>キーが押されているかどうか</returns>
        public bool GetKey(KeyCode key, SceneState target)
        {
            // 現在の状態が許可された状態であるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(new[] { target })) return false;

            // キー入力をチェック
            return Input.GetKey(key);
        }
        
        /// <summary>
        ///     キー入力を取得する。
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <param name="targets">入力を許す状態配列</param>
        /// <returns>キーが押されているかどうか</returns>
        public bool GetKey(KeyCode key, IEnumerable<SceneState> targets)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(targets)) return false;

            // キー入力をチェック
            return Input.GetKey(key);
        }

        #endregion

        #region GetKeyDown

        /// <summary>
        ///     キー入力状態を取得する
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <returns>キーが押された瞬間かどうか</returns>
        public bool GetKeyDown(KeyCode key)
        {
            // キー入力をチェック
            return Input.GetKeyDown(key);
        }

        /// <summary>
        ///     キー入力状態を取得する
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <param name="target">入力を許す状態</param>
        /// <returns>キーが押された瞬間かどうか</returns>
        public bool GetKeyDown(KeyCode key, SceneState target)
        {
            // 現在の状態が許可された状態であるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(new[] { target })) return false;

            // キー入力をチェック
            return Input.GetKeyDown(key);
        }

        /// <summary>
        ///     キー入力を取得する。
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <param name="targets">入力を許す状態配列</param>
        /// <returns>キーが押された瞬間かどうか</returns>
        public bool GetKeyDown(KeyCode key, IEnumerable<SceneState> targets)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(targets)) return false;

            // キー入力をチェック
            return Input.GetKeyDown(key);
        }

        #endregion

        #region GetMouseButton

        /// <summary>
        ///     マウスボタン入力を取得する。
        /// </summary>
        /// <param name="button">チェックするマウスボタン</param>
        /// <returns>マウスボタンが押されているかどうか</returns>
        public bool GetMouseButton(int button)
        {
            // マウスボタン入力をチェック
            return Input.GetMouseButton(button);
        }

        /// <summary>
        ///     マウスボタン入力を取得する。
        /// </summary>
        /// <param name="button">チェックするマウスボタン</param>
        /// <param name="target">入力を許す状態</param>
        /// <returns>マウスボタンが押されているかどうか</returns>
        public bool GetMouseButton(int button, SceneState target)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(new[] { target })) return false;

            // マウスボタン入力をチェック
            return Input.GetMouseButton(button);
        }

        /// <summary>
        ///     マウスボタン入力を取得する。
        /// </summary>
        /// <param name="button">チェックするマウスボタン</param>
        /// <param name="targets">入力を許す状態配列</param>
        /// <returns>マウスボタンが押されているかどうか</returns>
        public bool GetMouseButton(int button, IEnumerable<SceneState> targets)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(targets)) return false;

            // マウスボタン入力をチェック
            return Input.GetMouseButton(button);
        }

        #endregion

        #region GetAxis

        /// <summary>
        ///     * 入力軸の値を取得する。
        /// </summary>
        /// <param name="axisName">チェックする入力軸名</param>
        /// <returns>入力軸の値</returns>
        public float GetAxis(string axisName)
        {
            // 入力軸の値をチェック
            return Input.GetAxis(axisName);
        }

        /// <summary>
        ///     * 入力軸の値を取得する。
        /// </summary>
        /// <param name="axisName">チェックする入力軸名</param>
        /// <param name="target">入力を許す状態</param>
        /// <returns>入力軸の値</returns>
        public float GetAxis(string axisName, SceneState target)
        {
            // 現在の状態が許可された状態であるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(new[] { target })) return 0f;

            // 入力軸の値をチェック
            return Input.GetAxis(axisName);
        }

        /// <summary>
        ///     * 入力軸の値を取得する。
        /// </summary>
        /// <param name="axisName">チェックする入力軸名</param>
        /// <param name="targets">入力を許す状態配列</param>
        /// <returns>入力軸の値</returns>
        public float GetAxis(string axisName, IEnumerable<SceneState> targets)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(targets)) return 0f;

            // 入力軸の値をチェック
            return Input.GetAxis(axisName);
        }

        #endregion

        private bool CheckState(IEnumerable<SceneState> targets)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            foreach (var target in targets)
                if (StateContext.CurrentState.StateName == target)
                    return true;

            return false;
        }
    }
}