#region

using System;
using UnityEngine;

#endregion

namespace Lib.DataClass.Audio
{
    /// <summary>
    ///     SE用の識別子とAudioClipの組（Inspectorシリアライズ用）
    /// </summary>
    [Serializable]
    public class SeClipData
    {
        [Header("識別子")]
        [SerializeField]
        private string id;

        [Header("クリップ")]
        [SerializeField]
        private AudioClip clip;

        public string Id => id;

        public AudioClip Clip => clip;
    }
}
