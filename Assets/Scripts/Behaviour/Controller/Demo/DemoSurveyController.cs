#region

using Behaviour.Controller.General;
using Behaviour.Trigger;
using Lib.DataClass.ForInspector;
using Lib.Logic;
using ScriptableObj.Setting;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.Controller.Demo
{
    /// <summary>
    ///     デモ時にのプレイ時間を計測し、一定時間経過またはステージクリアでアンケートシーンへ遷移するコントローラー
    /// </summary>
    public class DemoSurveyController : MonoBehaviour
    {
        #region Private Fields

        // プレイ時間
        private float _playTime;

        // ステージ１のクリアフラグ
        private bool _stage1Clear;

        // ステージ２のクリアフラグ
        private bool _stage2Clear;

        // end demoのフラグ
        private bool _endDemo;

        // 初期化フラグ
        private bool _initialized;

        // 入力取得用
        // ReSharper disable once InconsistentNaming
        private InputController Input;

        // ステージ１のシーン名
        private SceneObj Stage1Name => environmentSetting.StageScenes[0];

        // ステージ２のシーン名
        private SceneObj Stage2Name => environmentSetting.StageScenes[1];

        // アンケートシーン名
        private SceneObj SurveySceneName => environmentSetting.EndCardScene;

        #endregion

        #region Serialized Fields

        [SerializeField]
        private EnvironmentSetting environmentSetting;

        // デモワンプレイごとのプレイ時間[min]
        [SerializeField]
        private float demoPlayMin = 60f;

        #endregion

        #region Unity Methods

        private void Start()
        {
            // 存在確認
            var existingController =
                FindObjectsByType<DemoSurveyController>(FindObjectsSortMode.None);
            if (existingController.Length > 1)
            {
                // 既に存在する場合はこのオブジェクトを破棄
                Destroy(gameObject);
                return;
            }
            
            // 破棄させない
            DontDestroyOnLoad(gameObject);

            // シーン読み込み時に処理を追加
            SceneManager.sceneLoaded += (_, _) => OnSceneLoaded();
            SceneManager.sceneLoaded += (_, _) => Init();
        }


        private void Update()
        {
            // 初期化されていなければ終了
            if (!_initialized) return;
            
            // プレイ時間の更新
            _playTime += Time.deltaTime;

            const float minToSec = 60f;

            // デモプレイ時間を超えたら終了
            if (_playTime >= demoPlayMin * minToSec) EndDemo();

            // ステージクリアの確認
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.C))
                // デバッグ用にステージクリアを強制的に呼び出す
                OnStageClear();
#endif
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// デモ終了処理
        /// </summary>
        private void EndDemo()
        {
            // 既にデモが終了している場合は何もしない
            if (_endDemo) return;
            // デモ終了フラグを立てる
            _endDemo = true;

            const float delayForLoad = 2f;

            // 数秒待ってからアンケートへ遷移
            var waitCoroutine =
                GeneralUtils.DelayCoroutine(delayForLoad, () =>
                    SceneManager.LoadScene(SurveySceneName.SceneName));
            StartCoroutine(waitCoroutine);

            // 初期化フラグを元に戻す
            _initialized = false;
        }

        /// <summary>
        /// ステージクリア時の処理
        /// </summary>
        private void OnStageClear()
        {
            // シーン名取得
            var sceneName = SceneManager.GetActiveScene().name;

            // ステージ1クリア
            if (sceneName == Stage1Name.SceneName && !_stage1Clear)
                _stage1Clear = true;

            // ステージ2クリア
            if (sceneName == Stage2Name.SceneName && !_stage2Clear)
                _stage2Clear = true;

            // どちらかがクリアされたら終了処理
            if (_stage1Clear || _stage2Clear)
                EndDemo();
        }

        private void OnSceneLoaded()
        {
            // ゴールがあればコールバックを設定
            // シーンから探す
            var goal = FindFirstObjectByType<GoalTrigger>();
            // ゴールが無ければリターン
            if (goal != null)
                goal.AddOnGoal(OnStageClear); // ステージクリア時の処理を登録

            // 入力コントローラーを取得
            Input = FindFirstObjectByType<InputController>();
        }

        private void Init()
        {
            // 既に初期化されている場合は何もしない
            if (_initialized) return;

            // シーンがステージかどうかを確認
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != Stage1Name.SceneName && sceneName != Stage2Name.SceneName)
                return; // ステージ以外のシーンでは初期化しない

            // 初期化フラグを立てる
            _initialized = true;

            // 値を初期化
            _playTime = 0f;
            _stage1Clear = false;
            _stage2Clear = false;
            _endDemo = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     デモの初期化状態をリセットする
        /// </summary>
        public void ResetDemo()
        {
            // 初期化フラグを元に戻す
            _initialized = false;
        }

        #endregion
    }
}