#region

using System.Collections.Generic;
using Lib.DataClass.ForInspector;
using UnityEngine;

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
        private List<SceneObj> stageScenes = new();

        [SerializeField]
        private SceneObj endCardScene;

        public bool IsDevelopmentBuild => isDevelopmentBuild;

        public SceneObj TitleScene => titleScene;

        public List<SceneObj> StageScenes => stageScenes;

        public SceneObj EndCardScene => endCardScene;
    }
}