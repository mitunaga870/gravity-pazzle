#region

using System.IO;
using System.Threading.Tasks;
using InstantReplay;
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(StartRecordKey) && Input.GetKey(StopRecordModifierKey))
                _ = StopRecording();
        }

        private void OnDestroy()
        {
            _session?.Dispose();
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