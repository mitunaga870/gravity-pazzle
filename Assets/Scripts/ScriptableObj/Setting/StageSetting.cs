#region

using Lib.DataClass.ForInspector;
using UnityEngine;

#endregion

namespace ScriptableObj.Setting
{
    /// <summary>
    ///     ステージの情報を保持するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "ステージ設定", menuName = "ScriptableObj/設定/ステージ設定", order = 1)]
    public class StageSetting : ScriptableObject
    {
        [Header("表示名")]
        [SerializeField]
        private string displayName;

        public string DisplayName => displayName;

        [Header("クリア想定時間(分)")]
        [SerializeField]
        private int clearTimeMinutes;

        public int ClearTimeMinutes => clearTimeMinutes;

        [Header("ステージシーン")]
        [SerializeField]
        private SceneObj stageScene;

        public SceneObj StageScene => stageScene;
    }
}