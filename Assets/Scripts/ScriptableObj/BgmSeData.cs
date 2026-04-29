#region

using System.Collections.Generic;
using Lib.DataClass.Audio;
using UnityEngine;
using ScriptableObj.Setting;

#endregion

namespace ScriptableObj
{
    /// <summary>
    ///     BGM・SEのAudioClipをInspectorで登録し、アセットとして保存するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "BGM・SEデータ", menuName = "ScriptableObj/BGM・SEデータ", order = 2)]
    public class BgmSeData : ScriptableObject
    {
        [Header("BGM一覧")]
        [SerializeField]
        private List<BgmClipData> bgmList = new();

        [Header("SE一覧")]
        [SerializeField]
        private List<SeClipData> seList = new();

        public IReadOnlyList<BgmClipData> BgmList => bgmList;

        public IReadOnlyList<SeClipData> SeList => seList;

        private void OnValidate()
        {
            RemoveDuplicateTargetSceneTypes();
        }

        private void RemoveDuplicateTargetSceneTypes()
        {
            if (bgmList == null || bgmList.Count == 0) return;

            var usedSceneTypes = new HashSet<EnvironmentSceneType>();

            foreach (var bgmData in bgmList)
            {
                if (bgmData == null || bgmData.Id == null || bgmData.Id.Count == 0) continue;

                var uniqueSceneTypesPerEntry = new List<EnvironmentSceneType>();
                foreach (var sceneType in bgmData.Id)
                {
                    if (sceneType == EnvironmentSceneType.Unknown || usedSceneTypes.Add(sceneType))
                        uniqueSceneTypesPerEntry.Add(sceneType);
                }

                bgmData.Id.Clear();
                bgmData.Id.AddRange(uniqueSceneTypesPerEntry);
            }
        }

        /// <summary>
        ///     識別子に一致するBGMのクリップを返す（複数ある場合は先頭）
        /// </summary>
        public AudioClip GetBgmClip(EnvironmentSceneType targetSceneType)
        {
            foreach (var item in bgmList)
            {
                if (item != null && item.Id.Contains(targetSceneType)) return item.Clip;
            }

            foreach (var item in bgmList)
            {
                if (item != null && item.Id.Contains(EnvironmentSceneType.Unknown)) return item.Clip;
            }

            return null;
        }

        /// <summary>
        ///     識別子に一致するSEのクリップを返す（複数ある場合は先頭）
        /// </summary>
        public AudioClip GetSeClip(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (var item in seList)
            {
                if (item != null && item.Id == id) return item.Clip;
            }

            return null;
        }
    }
}
