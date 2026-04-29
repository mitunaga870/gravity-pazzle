#region

using Lib.DataClass.ForInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Behaviour.Controller.General.DontDestoroy;

#endregion

namespace Behaviour.UI
{
    /// <summary>
    ///     指定下シーンにクリックで移動
    /// </summary>
    [RequireComponent(typeof(Button))]
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
            button.onClick.AddListener(() =>
            {
                var soundController = SoundController.Instance;
                if (soundController != null)
                {
                    soundController.PlaySe("SceneTransition");
                }

                SceneManager.LoadScene(targetScene);
            });
        }

        /// <summary>
        ///     ターゲットシーンを設定
        /// </summary>
        /// <param name="scene"></param>
        public void SetTargetScene(SceneObj scene)
        {
            targetScene = scene;
        }
    }
}