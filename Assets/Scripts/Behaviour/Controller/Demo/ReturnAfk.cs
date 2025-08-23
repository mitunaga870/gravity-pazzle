#region

using Behaviour.Controller.General;
using Lib.DataClass.ForInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Behaviour.Controller.Demo
{
    /// <summary>
    ///     運営用に、AFK時に指定シーンに戻るためのクラス
    /// </summary>
    public class ReturnAfk : MonoBehaviour
    {
        // 戻るシーン
        [SerializeField]
        private SceneObj returnScene;

        // AFK閾値[sec]
        [SerializeField]
        private float afkThreshold = 30f;

        // 入力コントローラー
        [SerializeField]
        private InputController Input;

        // AFK時間計測用
        private float _afkTime;

        private void Update()
        {
            // AFK時間を計測
            if (Input.InputString.Length > 0 || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                _afkTime = 0f; // 入力があった場合はAFK時間をリセット
            else
                _afkTime += Time.deltaTime; // 入力がなかった場合はAFK時間を更新

            // AFK時間が閾値を超えたら初期に戻す
            if (_afkTime >= afkThreshold) Reset();
        }

        private void Reset()
        {
            // 初期化
            _afkTime = 0f;

            // アンケート制御があればそれもリセット
            var surveyController = FindFirstObjectByType<DemoSurveyController>();
            if (surveyController != null)
                surveyController.ResetDemo();


            // シーンに遷移
            SceneManager.LoadScene(returnScene);
        }
    }
}