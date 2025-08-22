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
        /// <summary>
        ///     キー入力を取得する。
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <param name="targets">入力を許す状態配列</param>
        /// <param name="context">状態コンテキスト</param>
        /// <returns>キーが押されているかどうか</returns>
        public bool GetKey(KeyCode key, IEnumerable<ISceneState> targets, SceneStateContext context)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(targets, context)) return false;

            // キー入力をチェック
            return Input.GetKey(key);
        }

        /// <summary>
        ///     キー入力を取得する。
        /// </summary>
        /// <param name="key">チェックするキー</param>
        /// <param name="targets">入力を許す状態配列</param>
        /// <param name="context">状態コンテキスト</param>
        /// <returns>キーが押された瞬間かどうか</returns>
        public bool GetKeyDown(KeyCode key, IEnumerable<ISceneState> targets, SceneStateContext context)
        {
            // 現在の状態が許可された状態のいずれかであるかをチェック
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!CheckState(targets, context)) return false;

            // キー入力をチェック
            return Input.GetKeyDown(key);
        }

        private bool CheckState(IEnumerable<ISceneState> targets, SceneStateContext context)
        {
            // 現在の状態がnullの場合は、全ての状態で許可する
            if (context?.CurrentState == null) return true;

            // 現在の状態が許可された状態のいずれかであるかをチェック
            foreach (var target in targets)
                if (context.CurrentState.GetType() == target.GetType())
                    return true;

            return false;
        }
    }
}