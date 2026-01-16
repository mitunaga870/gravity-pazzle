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
            
            // 現状のシーンを再読み込み
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}