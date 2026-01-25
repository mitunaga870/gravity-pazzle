#region

using Behaviour.Controller.General.DontDestoroy;
using Lib.Logic;
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
        private SceneSelectButton startButton; // スタートボタン

        [SerializeField]
        private SceneSelectButton creditButton; // クレジットボタン
        
        
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
            var stageSelectScene = EnvironmentSetting.StageSelectScene;
            var creditScene = EnvironmentSetting.CreditScene;

            startButton.SetTargetScene(stageSelectScene);
            creditButton.SetTargetScene(creditScene);
        }
        
        public void QuitGame()
        {
            GeneralUtils.QuitGame();
        }
    }
}