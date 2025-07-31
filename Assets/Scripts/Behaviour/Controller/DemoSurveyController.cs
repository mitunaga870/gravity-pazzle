#region

using Behaviour.Trigger;
using Lib.DataClass.ForInspector;
using Lib.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.Controller
{
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

        #endregion

        #region Serialized Fields

        // デモワンプレイごとのプレイ時間[min]
        [SerializeField]
        private float demoPlayMin = 60f;

        // ステージの名前
        [SerializeField]
        private SceneObj stage1Name;

        [SerializeField]
        private SceneObj stage2Name;

        [SerializeField]
        private SceneObj surveySceneName;

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
            
            // 初期化
            _playTime = 0f;
            _stage1Clear = false;
            _stage2Clear = false;

            // 破棄させない
            DontDestroyOnLoad(gameObject);

            // シーン読み込み時に処理を追加
            SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded();
        }


        private void Update()
        {
            // プレイ時間の更新
            _playTime += Time.deltaTime;

            // デモプレイ時間を超えたら終了
            if (_playTime >= demoPlayMin * 60f) EndDemo();
            // else Debug.Log(_playTime + _stage1Clear.ToString() + _stage2Clear);

            // ステージクリアの確認
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.C))
                // デバッグ用にステージクリアを強制的に呼び出す
                OnStageClear();
#endif
        }

        #endregion

        #region Private Methods

        /**
         * デモ終了処理
         */
        private void EndDemo()
        {
            // 既にデモが終了している場合は何もしない
            if (_endDemo) return;
            // デモ終了フラグを立てる
            _endDemo = true;

            // 数秒待ってからアンケートへ遷移
            var waitCoroutine =
                GeneralUtils.DelayCoroutine(2f, () =>
                    SceneManager.LoadScene(surveySceneName.SceneName));
            StartCoroutine(waitCoroutine);

            // 値を初期化
            _playTime = 0f;
            _stage1Clear = false;
            _stage2Clear = false;
        }

        /**
         * ステージクリア時の処理
         */
        private void OnStageClear()
        {
            // シーン名取得
            var sceneName = SceneManager.GetActiveScene().name;

            // ステージ1クリア
            if (sceneName == stage1Name.SceneName && !_stage1Clear)
                _stage1Clear = true;

            // ステージ2クリア
            if (sceneName == stage2Name.SceneName && !_stage2Clear)
                _stage2Clear = true;

            // 両方のステージがクリアされたらデモ終了
            if (_stage1Clear && _stage2Clear)
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
        }

        #endregion
    }
}