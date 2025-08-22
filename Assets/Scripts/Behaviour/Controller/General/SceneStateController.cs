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
    }
}