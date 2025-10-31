#region

using System.Collections.Generic;
using System.Linq;
using Lib.DataClass.ForInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace ScriptableObj.Setting
{
    [CreateAssetMenu(fileName = "環境設定", menuName = "ScriptableObj/設定/環境設定", order = 1)]
    public class EnvironmentSetting : ScriptableObject
    {
        [Header("開発用設定")]
        [SerializeField]
        private bool isDevelopmentBuild;

        [Header("ビルド設定")]
        [SerializeField]
        private SceneObj titleScene;

        [SerializeField]
        private List<StageSetting> stages = new();

        [SerializeField]
        private SceneObj endCardScene;

        public bool IsDevelopmentBuild => isDevelopmentBuild;

        public SceneObj TitleScene => titleScene;

        public List<StageSetting> Stages => stages;

        public SceneObj EndCardScene => endCardScene;

        public (StageSetting, int) GetFromCurScene()
        {
            var curSceneName = SceneManager.GetActiveScene().name;

            foreach (var (stage, index) in stages.Select((value, i) => (value, i)))
                if (stage.StageScene.SceneName == curSceneName)
                    return (stage, index);

            throw new KeyNotFoundException($"Scene '{curSceneName}' not found in EnvironmentSetting stages.");
        }
    }
}