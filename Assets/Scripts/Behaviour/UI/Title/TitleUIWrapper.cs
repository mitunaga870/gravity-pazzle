#region

using System.Collections.Generic;
using Behaviour.Controller.General.DontDestoroy;
using ScriptableObj.Setting;
using UnityEngine;

#endregion

namespace Behaviour.UI.Title
{
    /// <summary>
    ///     タイトルのUIに関するラッパークラス
    /// </summary>
    public class TitleUIWrapper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private List<SceneSelectButton> stageButtons = new();
        
        #endregion

        private static EnvironmentSetting EnvironmentSetting
        {
            get
            {
                var setting = SettingDataController.Instance.EnvironmentSetting;
                return setting;
            }
        }

        private void Start()
        {
            // ステージ数とボタン数が一致しない場合はエラーを出す
            if (EnvironmentSetting.StageScenes.Count != stageButtons.Count)
            {
                Debug.LogError("Stage scenes count and stage buttons count do not match.");
                return;
            }

            // ステージ選択ボタンにステージシーンを割り当てる
            for (var i = 0; i < stageButtons.Count; i++)
                stageButtons[i].SetTargetScene(EnvironmentSetting.StageScenes[i]);
        }
    }
}