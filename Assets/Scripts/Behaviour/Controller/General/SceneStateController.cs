#region

using Lib.State.Scene;
using UnityEngine;

#endregion

namespace Behaviour.Controller.General
{
    public class SceneStateController : MonoBehaviour
    {
        /// <summary>
        ///     状態操作コンストラクタ
        /// </summary>
        public SceneStateContext Context { get; protected set; }

        /// <summary>
        ///     初期状態
        /// </summary>
        [SerializeField]
        private SceneState initialState;

        #region Unity Methods

        private void Start()
        {
            // コンテキストの初期化
            Context = new SceneStateContext(
                SceneStateUtils.GenerateState(initialState));
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     *     シーン状態を変更する。
        /// </summary>
        /// <param name="next">次の状態</param>
        /// <param name="forceChange">強制的に変更するかどうか</param>
        public void ChangeSceneState(SceneState next, bool forceChange = false)
        {
            // 状態を変更する
            Context.Change(SceneStateUtils.GenerateState(next), forceChange);
        }

        #endregion
    }
}