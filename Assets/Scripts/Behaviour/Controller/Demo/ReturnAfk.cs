#region

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

        // AFK時間計測用
        private float _afkTime;

        private void Update()
        {
            // AFK時間を計測
            if (Input.inputString.Length > 0 || Input.anyKeyDown)
                _afkTime = 0f; // 入力があった場合はAFK時間をリセット
            else
                _afkTime += Time.deltaTime; // 入力がなかった場合はAFK時間を更新

            // AFK時間が閾値を超えたら指定シーンに戻る
            if (_afkTime >= afkThreshold) SceneManager.LoadScene(returnScene);
        }
    }
}