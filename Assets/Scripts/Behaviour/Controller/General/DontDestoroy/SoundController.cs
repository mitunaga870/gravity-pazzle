#region

using System;
using ScriptableObj;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using ScriptableObj.Setting;

#endregion

namespace Behaviour.Controller.General.DontDestoroy
{
    /// <summary>
    ///     BGM・SE 等の再生制御を行う DontDestroy コントローラー。
    ///     再生は AudioMixer の Bgm / Se グループへルーティングし、露出パラメータ（MasterVolume, BgmVolume, SeVolume）で音量を制御する。
    /// </summary>
    public class SoundController : AudioMixerControllerBase
    {
        #region Singleton Implementation

        public static SoundController Instance { get; private set; }

        private SoundController()
        {
        }

        #endregion

        #region Serialized Fields

        [SerializeField]
        private BgmSeData bgmSeData;

        #endregion

        #region Private Fields

        private AudioSource _bgmSource;
        private AudioSource _seSource;
        
        private SettingDataController _settingDataController;
        private EnvironmentSceneType _currentEnvironmentSceneType = EnvironmentSceneType.Unknown;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }

        private void OnDestroy()
        {
            if (Instance != this) return;

            Instance = null;
        }

        private void Start()
        {
            if (!TryResolveSettingDataController()) return;

            SyncMixerVolumeFromSettings();
            InitializeBgm();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!TryResolveSettingDataController()) return;

            SyncMixerVolumeFromSettings();
            InitializeBgm();
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     識別子に対応する BGM をループ再生する（ミキサー Bgm 経由）
        /// </summary>
        public void PlayBgm(EnvironmentSceneType targetSceneType)
        {
            if (bgmSeData == null || _bgmSource == null) return;

            var bgmClip = bgmSeData.GetBgmClip(targetSceneType);
            if (bgmClip == null) return;

            if (_bgmSource.isPlaying && _bgmSource.clip == bgmClip) return;

            _bgmSource.clip = bgmClip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        /// <summary>
        ///     BGM を停止する
        /// </summary>
        public void StopBgm()
        {
            if (_bgmSource == null) return;

            _bgmSource.Stop();
            _bgmSource.clip = null;
        }

        /// <summary>
        ///     識別子に対応する SE をワンショット再生する（ミキサー Se 経由）
        /// </summary>
        public void PlaySe(string seId)
        {
            if (_seSource == null)
            {
                Debug.LogError($"SoundController: SE用AudioSourceが未初期化のため、SE '{seId}' を再生できません。");
                return;
            }

            if (bgmSeData == null)
            {
                Debug.LogError($"SoundController: BgmSeData が未設定のため、SE '{seId}' を再生できません。");
                return;
            }

            if (string.IsNullOrEmpty(seId))
            {
                Debug.LogError("SoundController: 空またはnullのSE IDが指定されました。");
                return;
            }

            var seClip = bgmSeData.GetSeClip(seId);
            if (seClip == null)
            {
                Debug.LogError($"SoundController: SE ID '{seId}' に対応するクリップが見つかりません。");
                return;
            }

            _seSource.PlayOneShot(seClip);
        }

        /// <summary>
        ///     任意のクリップを BGM 用バスでループ再生する
        /// </summary>
        public void PlayBgmClip(AudioClip clip)
        {
            if (clip == null || _bgmSource == null) return;

            if (_bgmSource.isPlaying && _bgmSource.clip == clip) return;

            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        /// <summary>
        ///     イベント識別子に対応するBGMを再生する
        /// </summary>
        public void PlayEventBgm(EventBgmType eventType, bool loop = false)
        {
            if (bgmSeData == null || _bgmSource == null || eventType == EventBgmType.Unknown) return;

            var eventBgmClip = ResolveEventBgmClip(eventType);
            if (eventBgmClip == null)
            {
                Debug.LogWarning($"SoundController: EventBgmType '{eventType}' に対応するクリップが見つかりません。");
                return;
            }

            if (_bgmSource.isPlaying && _bgmSource.clip == eventBgmClip && _bgmSource.loop == loop) return;

            _bgmSource.clip = eventBgmClip;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }

        /// <summary>
        ///     現在のシーン種別に対応するBGMへ戻す
        /// </summary>
        public void ResumeSceneBgm()
        {
            if (_settingDataController == null)
            {
                _settingDataController = SettingDataController.Instance;
                if (_settingDataController == null) return;
            }

            _currentEnvironmentSceneType = _settingDataController.EnvironmentSetting.GetCurrentEnvironmentSceneType();
            PlayBgm(_currentEnvironmentSceneType);
        }

        /// <summary>
        ///     任意のクリップを SE 用バスでワンショット再生する
        /// </summary>
        public void PlaySeClip(AudioClip clip)
        {
            if (clip == null || _seSource == null) return;

            _seSource.PlayOneShot(clip);
        }

        /// <summary>
        ///     シーン遷移SE再生後の待機秒数を環境設定から取得する
        /// </summary>
        public static float GetSceneTransitionDelaySeconds()
        {
            var settingController = SettingDataController.Instance;
            var environmentSetting = settingController != null ? settingController.EnvironmentSetting : null;
            return environmentSetting != null ? environmentSetting.SceneTransitionDelaySeconds : 0f;
        }

        /// <summary>
        ///     AudioMixer の露出 float パラメータを線形 0〜1 から dB に変換して設定する（SettingDataController と同じ換算）
        /// </summary>
        public void SetMixerVolumeLinear(string exposedParameterName, float linearVolume)
        {
            if (Mixer == null || string.IsNullOrEmpty(exposedParameterName)) return;

            linearVolume = Mathf.Clamp01(linearVolume);
            var db = linearVolume <= 0.0001f ? -80f : Mathf.Log10(linearVolume) * 20f;
            Mixer.SetFloat(exposedParameterName, db);
        }

        #endregion

        #region Private Methods
        
        private void InitializeBgm()
        {
            if (!TryResolveSettingDataController()) return;

            _currentEnvironmentSceneType = _settingDataController.EnvironmentSetting.GetCurrentEnvironmentSceneType();
            PlayBgm(_currentEnvironmentSceneType);
        }

        private bool TryResolveSettingDataController()
        {
            if (_settingDataController != null) return true;

            _settingDataController = SettingDataController.Instance;
            if (_settingDataController != null) return true;

            Debug.LogError("SoundController: SettingDataController が未設定のため、BGMを再生できません。");
            return false;
        }

        private void SyncMixerVolumeFromSettings()
        {
            _settingDataController.ApplySettings();
        }

        private void SetupAudioSources()
        {
            if (Mixer == null)
            {
                Debug.LogWarning("SoundController: AudioMixer が未設定のため、ミキサー経由の再生ができません。");
                return;
            }

            var bgmGroup = FindFirstMixerGroup("Bgm", "Master/Bgm");
            var seGroup = FindFirstMixerGroup("Se", "Master/Se");

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.spatialBlend = 0f;
            if (bgmGroup != null) _bgmSource.outputAudioMixerGroup = bgmGroup;

            _seSource = gameObject.AddComponent<AudioSource>();
            _seSource.playOnAwake = false;
            _seSource.loop = false;
            _seSource.spatialBlend = 0f;
            if (seGroup != null) _seSource.outputAudioMixerGroup = seGroup;
        }

        private AudioClip ResolveEventBgmClip(EventBgmType eventType)
        {
            if (bgmSeData == null || eventType == EventBgmType.Unknown) return null;
            return bgmSeData.GetEventBgmClip(eventType);
        }

        /// <summary>
        ///     FindMatchingGroups の結果から先頭を取る。短い名前で見つからない場合はフルパスを試す。
        /// </summary>
        private AudioMixerGroup FindFirstMixerGroup(string shortPath, string fullPathFallback)
        {
            var found = Mixer.FindMatchingGroups(shortPath);
            if (found is { Length: > 0 }) return found[0];

            found = Mixer.FindMatchingGroups(fullPathFallback);
            return found is { Length: > 0 } ? found[0] : null;
        }

        #endregion
    }
}
