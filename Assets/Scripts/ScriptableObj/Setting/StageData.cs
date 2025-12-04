#region

using System;
using Lib.DataClass.ForInspector;
using UnityEngine;

#endregion

namespace ScriptableObj.Setting
{
    /// <summary>
    ///     ステージの情報を保持するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "ステージ情報", menuName = "ScriptableObj/パラメータ設定/ステージ情報", order = 1)]
    public class StageData : ScriptableObject
    {
        [Header("ステージID")]
        [SerializeField]
        private string stageId = Guid.NewGuid().ToString();

        public string StageId => stageId;
        
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

        [Header("ステージサムネ")]
        [SerializeField]
        private Sprite stageThumbnail;

        public Sprite StageThumbnail => stageThumbnail;
    }
}