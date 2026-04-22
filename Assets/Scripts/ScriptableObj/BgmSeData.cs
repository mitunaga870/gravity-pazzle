#region

using System.Collections.Generic;
using Lib.DataClass.Audio;
using UnityEngine;

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

        /// <summary>
        ///     識別子に一致するBGMのクリップを返す（複数ある場合は先頭）
        /// </summary>
        public AudioClip GetBgmClip(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (var item in bgmList)
            {
                if (item != null && item.Id == id) return item.Clip;
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
