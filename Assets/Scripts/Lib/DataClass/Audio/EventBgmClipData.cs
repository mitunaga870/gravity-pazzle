#region

using System;
using ScriptableObj.Setting;
using UnityEngine;

#endregion

namespace Lib.DataClass.Audio
{
    /// <summary>
    ///     イベントBGM用の識別子とAudioClipの組（Inspectorシリアライズ用）
    /// </summary>
    [Serializable]
    public class EventBgmClipData
    {
        [Header("イベントBGM識別子")]
        [SerializeField]
        private EventBgmType id = EventBgmType.Unknown;

        [Header("クリップ")]
        [SerializeField]
        private AudioClip clip;

        public EventBgmType Id => id;

        public AudioClip Clip => clip;
    }
}
