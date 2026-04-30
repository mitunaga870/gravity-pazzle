#region

using Lib.DataClass.ForInspector;
using Lib.Logic;
using Behaviour.Controller.General.DontDestoroy;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.Controller.General
{
    /// <summary>
    ///     自動で指定した時間後にシーンを変更するコンポーネント
    /// </summary>
    public class AutoSceneChanger : MonoBehaviour
    {
        // Scene変更までの時間[sec]
        [SerializeField]
        private float changeTime = 60f;

        // 次のシーン名
        [SerializeField]
        private SceneObj nextSceneName;

        private void Start()
        {
            // シーン変更までの時間を待つ
            StartCoroutine(GeneralUtils.DelayCoroutine(
                changeTime,
                () =>
                {
                    var soundController = SoundController.Instance;
                    if (soundController != null)
                    {
                        soundController.PlaySe("SceneTransition");
                    }

                    StartCoroutine(LoadNextSceneWithSceneTransitionSe());
                }));
        }

        private System.Collections.IEnumerator LoadNextSceneWithSceneTransitionSe()
        {
            yield return new WaitForSeconds(SoundController.GetSceneTransitionDelaySeconds());

            SceneManager.LoadScene(nextSceneName.SceneName);
        }
    }
}