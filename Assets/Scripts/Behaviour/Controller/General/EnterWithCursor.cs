#region

using UnityEngine;

#endregion

namespace Behaviour.Controller.General
{
    /// <summary>
    ///     ゲーム開始時にカーソルを表示するコンポーネント
    /// </summary>
    public class EnterWithCursor : MonoBehaviour
    {
        private void Start()
        {
            // ゲーム開始時にカーソルをロックしない
            Cursor.lockState = CursorLockMode.None;
            // カーソル表示
            Cursor.visible = true;
        }
    }
}