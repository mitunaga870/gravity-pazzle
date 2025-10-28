#region

using Lib.DataClass.Settings.GravSelectMethod;
using UnityEngine;
using Resolution = ScriptableObj.Setting.Resolution;

#endregion

namespace ScriptableObj
{
    /// <summary>
    ///     初期ユーザー設定を保存するスクリプタブルオブジェクト
    /// </summary>
    [CreateAssetMenu(fileName = "初期ユーザー設定", menuName = "ScriptableObj/設定/初期ユーザー設定", order = 0)]
    public class InitUserSettings : ScriptableObject
    {
        #region Serialized Fields

        [Header("ディスプレイ設定")]
        [SerializeField]
        private Resolution resolutionPreset;
        
        [Header("音声設定")]
        [SerializeField]
        [Range(0f, 1f)]
        private float masterVolume = 1.0f;

        [SerializeField]
        [Range(0f, 1f)]
        private float bgmVolume = 0.8f;

        [SerializeField]
        [Range(0f, 1f)]
        private float seVolume = 0.8f;

        [Header("ゲーム設定")]
        [SerializeField]
        private bool showTutorial = true;

        [SerializeField]
        private GravSelectMethod gravSelectMethod = Lib.DataClass.Settings.GravSelectMethod.GravSelectMethod.Mouse;

        #endregion

        #region Accessors

        public int ResolutionWidth => resolutionPreset.Width;
        public int ResolutionHeight => resolutionPreset.Height;
        public FullScreenMode Fullscreen => resolutionPreset.FullscreenMode;
        public float MasterVolume => masterVolume;
        public float BgmVolume => bgmVolume;
        public float SeVolume => seVolume;
        public bool ShowTutorial => showTutorial;

        public IGravSelectMethod GravSelectMethod => gravSelectMethod.ToGravSelectMethod();

        #endregion
    }
}