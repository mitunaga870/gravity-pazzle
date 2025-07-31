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
            // カーソル表示
            Cursor.visible = true;
        }
    }
}