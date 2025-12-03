#region

using System.IO;
using System.Threading.Tasks;
using InstantReplay;
using TMPro;
using UnityEditor;
using UnityEngine;

#endregion

namespace Behaviour.Controller.General.DontDestoroy
{
    /// <summary>
    ///     インスタントリプレイの制御を行うクラス
    ///     外部参照は行わないが、多重生成を防ぐためにシングルトンパターンで実装
    /// </summary>
    public class InstantReplayController : MonoBehaviour
    {
        private RealtimeInstantReplaySession _session;

        private const KeyCode StopRecordModifierKey = KeyCode.LeftControl;
        private const KeyCode StartRecordKey = KeyCode.T;

        [SerializeField]
        private TMP_Text keyLog;

        [SerializeField]
        private TMP_Text debugLog;

        #region Singleton Implementation

        private static InstantReplayController instance;


        private InstantReplayController()
        {
        }

        #endregion

        private void Start()
        {
            // 開発ビルドの時以外はオブジェクトを破棄する
            if (!SettingDataController.Instance.EnvironmentSetting.IsDevelopmentBuild)
            {
                Destroy(gameObject);
                return;
            }

            // 多重生成を防ぐ
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // シングルトンパターンの実装
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            _session = RealtimeInstantReplaySession.CreateDefault();

            // ログをテキストに表示
            Application.logMessageReceived += UpdateDebugLog;
        }

        private void Update()
        {
            // キーログを表示
            var keyString = $"Key: {Input.inputString}";
            keyLog.text = keyString;
            
            
            if (Input.GetKeyDown(StartRecordKey) && Input.GetKey(StopRecordModifierKey))
                _ = StopRecording();
        }

        private void OnDestroy()
        {
            _session?.Dispose();
            // ログの購読を解除
            Application.logMessageReceived -= UpdateDebugLog;
        }

        /// <summary>
        ///     デバッグログをUIに表示する
        ///     Application.logMessageReceivedのコールバックとして使用する
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void UpdateDebugLog(string condition, string stackTrace, LogType type)
        {
            var message = $"[{type}] {condition}\n{stackTrace}";

            //100文字以上になったら切り詰める
            if (message.Length > 100)
                message = message.Substring(0, 100) + "...(truncated)";

            debugLog.text = message;
        }

        /// <summary>
        ///     録画を停止し、ログ等とまとめて保存する
        /// </summary>
        private async Task StopRecording()
        {
            Debug.Log("Stopping Instant Replay recording...");
            var savedPath = await _session.StopAndExportAsync();

            // 動画を移動
            var dest = Path.Combine(Application.dataPath, "InstantReplay", Path.GetFileName(savedPath));
            if (!Directory.Exists(Path.GetDirectoryName(dest)))
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            FileUtil.MoveFileOrDirectory(savedPath, dest);
            Debug.Log($"Instant Replay saved to: {dest}");
            
            _session = RealtimeInstantReplaySession.CreateDefault();
        }
    }
}