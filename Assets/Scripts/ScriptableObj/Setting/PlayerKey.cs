#region

using Unity.VisualScripting;
using UnityEngine;

#endregion

namespace ScriptableObj.Setting
{
    /// <summary>
    ///     プレイヤーのキー設定を保持するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerKey", menuName = "ScriptableObj/設定/プレイヤーのキー設定", order = 3)]
    public class PlayerKey : ScriptableObject
    {
        #region serialize field

        [Header("移動キー設定")]
        [SerializeField]
        private KeyCode moveForwardKey = KeyCode.W;

        [SerializeField]
        private KeyCode moveBackwardKey = KeyCode.S;

        [SerializeField]
        private KeyCode moveLeftKey = KeyCode.A;

        [SerializeField]
        private KeyCode moveRightKey = KeyCode.D;

        [Header("重力指定キー設定")]
        [SerializeField]
        private KeyCode setPlayerGravKey = KeyCode.E;

        [SerializeField]
        private MouseButton setObjGravButton = MouseButton.Left;

        [Header("重力方向変更キー設定")]
        [SerializeField]
        private MouseButton changeGravDirectionMouseButton = MouseButton.Right;

        [SerializeField]
        private KeyCode changeGravDirectionModifierKey = KeyCode.LeftShift;

        [SerializeField]
        private KeyCode changeGravDirectionToTopKey = KeyCode.Space;

        [SerializeField]
        private KeyCode changeGravDirectionToBottomKey = KeyCode.LeftControl;

        #endregion

        #region accessor

        public KeyCode MoveForwardKey => moveForwardKey;
        public KeyCode MoveBackwardKey => moveBackwardKey;
        public KeyCode MoveLeftKey => moveLeftKey;
        public KeyCode MoveRightKey => moveRightKey;

        public KeyCode SetPlayerGravKey => setPlayerGravKey;
        public MouseButton SetObjGravButton => setObjGravButton;

        public MouseButton ChangeGravDirectionMouseButton => changeGravDirectionMouseButton;
        public KeyCode ChangeGravDirectionModifierKey => changeGravDirectionModifierKey;
        public KeyCode ChangeGravDirectionToTopKey => changeGravDirectionToTopKey;
        public KeyCode ChangeGravDirectionToBottomKey => changeGravDirectionToBottomKey;

        #endregion
    }
}