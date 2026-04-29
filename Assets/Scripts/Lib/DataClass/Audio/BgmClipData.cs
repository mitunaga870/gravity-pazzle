#region

using System;
using UnityEngine;
using ScriptableObj.Setting;
using System.Collections.Generic;

#endregion

namespace Lib.DataClass.Audio
{
    /// <summary>
    ///     BGM用の識別子とAudioClipの組（Inspectorシリアライズ用）
    /// </summary>
    [Serializable]
    public class BgmClipData : ISerializationCallbackReceiver
    {
        [Header("BGMが再生されるシーン一覧")]
        [SerializeField]
        private List<EnvironmentSceneType> targetSceneType = new();

        [Header("クリップ")]
        [SerializeField]
        private AudioClip clip;

        public List<EnvironmentSceneType> Id => targetSceneType;

        public AudioClip Clip => clip;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (targetSceneType == null || targetSceneType.Count <= 1) return;

            var uniqueIds = new HashSet<EnvironmentSceneType>();
            var normalized = new List<EnvironmentSceneType>();

            foreach (var sceneType in targetSceneType)
            {
                if (sceneType == EnvironmentSceneType.Unknown || uniqueIds.Add(sceneType))
                {
                    normalized.Add(sceneType);
                    continue;
                }

                normalized.Add(EnvironmentSceneType.Unknown);
            }

            targetSceneType = normalized;
        }
    }
}
