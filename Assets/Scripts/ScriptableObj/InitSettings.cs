#region

using UnityEngine;

#endregion

namespace ScriptableObj
{
    /// <summary>
    ///     初期ユーザー設定を保存するスクリプタブルオブジェクト
    /// </summary>
    [CreateAssetMenu(fileName = "初期ユーザー設定", menuName = "ScriptableObj/設定", order = 0)]
    public class InitSettings : ScriptableObject
    {
        #region Serialized Fields

        [Header("ディスプレイ設定")]
        [SerializeField]
        private int resolutionWidth = 1920;

        [SerializeField]
        private int resolutionHeight = 1080;

        [SerializeField]
        private bool fullscreen = true;

        [SerializeField]
        private int targetDisplay;

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

        #endregion

        #region Accessors

        public int ResolutionWidth => resolutionWidth;
        public int ResolutionHeight => resolutionHeight;
        public bool Fullscreen => fullscreen;
        public int TargetDisplay => targetDisplay;
        public float MasterVolume => masterVolume;
        public float BgmVolume => bgmVolume;
        public float SeVolume => seVolume;
        public bool ShowTutorial => showTutorial;

        #endregion
    }
}