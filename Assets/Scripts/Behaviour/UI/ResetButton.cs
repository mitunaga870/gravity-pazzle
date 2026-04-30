using System;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using Lib.Logic.General;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Behaviour.UI
{
    /// <summary>
    /// テスト用として、セーブデータを消去するボタン
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ResetButton : MonoBehaviour
    {
        private void Start()
        {
            var button  = GetComponent<Button>();
            button.onClick.AddListener(Onclick);
        }

        private static void Onclick()
        {
            SaveUtils.DeleteAllPlayData();

            SettingDataController.Instance.ReloadAllData();
            PlayerDataController.Instance.ReloadAllData();

            // ステージの場合、保存を無効化する
            var instance = StageDataController.Instance;
            if (instance != null) instance.DontSaveOnDestroy = true;
            
            var instanceGo = new GameObject("ResetButtonSceneReloader");
            instanceGo.AddComponent<ResetButtonSceneReloader>();
        }
    }

    internal class ResetButtonSceneReloader : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(ReloadCurrentSceneWithSceneTransitionSe());
        }

        private System.Collections.IEnumerator ReloadCurrentSceneWithSceneTransitionSe()
        {
            var soundController = SoundController.Instance;
            if (soundController != null)
            {
                soundController.PlaySe("SceneTransition");
            }

            yield return new WaitForSeconds(SoundController.GetSceneTransitionDelaySeconds());

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Destroy(gameObject);
        }
    }
}