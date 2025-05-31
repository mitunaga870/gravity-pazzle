#region

using Lib.DataClass.ForInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

namespace Behaviour.UI
{
    /// <summary>
    ///     指定下シーンにクリックで移動
    /// </summary>
    public class SceneSelectButton : MonoBehaviour
    {
        [SerializeField]
        private SceneObj targetScene;

        private void Start()
        {
            // ボタンコンポーネントを取得
            var button = GetComponent<Button>();

            // ボタンが見つからない場合はエラーメッセージを表示
            if (button == null)
            {
                Debug.LogError("Button component not found on this GameObject.");
                return;
            }

            // ボタンにクリックイベントを追加
            button.onClick.AddListener(() => SceneManager.LoadScene(targetScene));
        }
    }
}