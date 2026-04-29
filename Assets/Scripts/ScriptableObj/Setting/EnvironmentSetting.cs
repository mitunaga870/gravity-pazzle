#region

using System;
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
        private SceneObj stageSelectScene;

        [SerializeField]
        private SceneObj creditScene;

        [SerializeField]
        private List<StageData> stages = new();

        [SerializeField]
        [Obsolete("EndCardSceneはデモ版専用のため、今後廃止予定です。")]
        private SceneObj endCardScene;

        [Header("サウンド設定")]
        [SerializeField]
        [Min(0f)]
        private float sceneTransitionDelaySeconds = 1f;

        public bool IsDevelopmentBuild => isDevelopmentBuild;

        public SceneObj TitleScene => titleScene;
        public SceneObj StageSelectScene => stageSelectScene;
        public SceneObj CreditScene => creditScene;

        public List<StageData> Stages => stages;

        [Obsolete("StageSelectSceneは今後廃止予定です。")]
        public SceneObj EndCardScene => endCardScene;

        public float SceneTransitionDelaySeconds => sceneTransitionDelaySeconds;

        public (StageData, int) GetFromCurScene()
        {
            var curSceneName = SceneManager.GetActiveScene().name;

            foreach (var (stage, index) in stages.Select((value, i) => (value, i)))
                if (stage.StageScene.SceneName == curSceneName)
                    return (stage, index);

            throw new KeyNotFoundException($"Scene '{curSceneName}' not found in EnvironmentSetting stages.");
        }
        
        public EnvironmentSceneType GetCurrentEnvironmentSceneType()
        {
            var curSceneName = SceneManager.GetActiveScene().name;
            if (titleScene.SceneName == curSceneName)
                return EnvironmentSceneType.Title;
            if (stageSelectScene.SceneName == curSceneName)
                return EnvironmentSceneType.StageSelect;
            if (creditScene.SceneName == curSceneName)
                return EnvironmentSceneType.Credit;
            if (stages.Any(stage => stage.StageScene.SceneName == curSceneName))
                return EnvironmentSceneType.Stage;
            return EnvironmentSceneType.Unknown;
        }
    }
    
    public enum EnvironmentSceneType
    {
        Unknown,
        Title,
        StageSelect,
        Credit,
        Stage
    }

    public enum EventBgmType
    {
        Unknown,
        BossAppear,
        StageClear,
        GameOver
    }
}
