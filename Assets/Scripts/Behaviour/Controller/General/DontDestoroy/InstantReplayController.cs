#region

using System;
using System.IO;
using System.Threading.Tasks;
using InstantReplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (!Debug.isDebugBuild)
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

            // 初期化処理
            try
            {
                _session = RealtimeInstantReplaySession.CreateDefault();

                // ログをテキストに表示
                Application.logMessageReceived += UpdateDebugLog;

                // 開発ログを非表示にする
                SceneManager.sceneLoaded += (_, _) => { Debug.developerConsoleVisible = false; };
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize InstantReplaySettings: {e}");
            }

        }

        private void FixedUpdate()
        {
            // キーログを表示
            var keyString = $"Key: {Input.inputString}";
            keyLog.text = keyString;
            
            
            if (Input.GetKeyDown(StartRecordKey) && Input.GetKey(StopRecordModifierKey))
                _ = StopRecording();
        }

        private async void OnDestroy()
        {
            try
            {
                if (_session != null)
                {
                    // 録画を停止してエクスポートその後破棄
                    var path = await _session.StopAndExportAsync();
                    File.Delete(path);

                    _session.Dispose();
                }

                // ログの購読を解除
                Application.logMessageReceived -= UpdateDebugLog;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while destroying InstantReplayController: {e}");
            }
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
            // 保存先ディレクトリの準備
            const string instantReplayDirName = "InstantReplay";
            var executeDirName = Path.GetDirectoryName(Application.dataPath);
            if (executeDirName == null)
            {
                Debug.LogError("Failed to get execute directory name.");
                return;
            }

            // 保存先ディレクトリの作成
            var instantReplayDir = Path.Combine(executeDirName, instantReplayDirName);
            if (!Directory.Exists(instantReplayDir))
                Directory.CreateDirectory(instantReplayDir);
            var thisReplayDir = Path.Combine(instantReplayDir,
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            if (!Directory.Exists(thisReplayDir))
                Directory.CreateDirectory(thisReplayDir);

            // 録画を停止してエクスポート
            var savedPath = await _session.StopAndExportAsync();
            var movieDest = Path.Combine(thisReplayDir, Path.GetFileName(savedPath));
            File.Move(savedPath, movieDest);
            Debug.Log($"Instant Replay saved to: {movieDest}");

            // ログファイルの保存
            var logDest = Path.Combine(thisReplayDir, "log.txt");
            var logLines = Application.consoleLogPath;
            File.Copy(logLines, logDest, true);
            Debug.Log($"Log file saved to: {logDest}");

            // 新しいセッションを開始
            _session = RealtimeInstantReplaySession.CreateDefault();
        }
    }
}