#region

using UnityEngine;
using UnityEngine.Video;

#endregion

namespace Behaviour.UI
{
    /**
     * スクリーンセーバーの実装クラス
     * リファレンス: https://takopa.atlassian.net/wiki/x/BADBAw
     */
    public class ScreenSaver : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float screenSaverTime = 60f; // スクリーンセーバーまでの時間（秒）

        [SerializeField]
        private GameObject screenSaverObj; // スクリーンセーバーのオブジェクト

        #endregion

        #region Private Fields

        // 放置時間
        private float _idleTime;

        // スクリーンセーバーが表示されているかどうか
        private bool _isScreenSaverActive;

        // スクリーンセーバーのオブジェクトの動画プレイヤー
        private VideoPlayer _videoPlayer;

        // スクリーンセーバーのオブジェクトが動画プレイヤーを持っているかどうか
        private bool _hasVideoPlayer;

        #endregion

        #region Unity Methods

        private void Start()
        {
            // スクリーンセーバーのオブジェクトが設定されていない場合はエラーを出す
            if (screenSaverObj == null) Debug.LogError("ScreenSaver object is not assigned.");

            // ビデオプレイヤーが設定されている場合は取得
            if (screenSaverObj.TryGetComponent<VideoPlayer>(out var videoPlayer))
            {
                _videoPlayer = videoPlayer; // ビデオプレイヤーを保存
                _hasVideoPlayer = true; // スクリーンセーバーオブジェクトが動画プレイヤーを持っている
            }

            // 初期化
            _idleTime = 0f; // 放置時間を初期化
            HideScreenSaver(true); // スクリーンセーバーを非表示にする
        }

        private void Update()
        {
            // マウスの動きやキー入力を検出
            if (Input.inputString.Length > 0 || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _idleTime = 0f; // 放置時間をリセット
                HideScreenSaver(); // スクリーンセーバーを非表示にする
            }
            else
            {
                _idleTime += Time.deltaTime; // 放置時間を更新

                // スクリーンセーバーの時間を超えたら表示
                if (_idleTime >= screenSaverTime) ShowScreenSaver();
            }
        }

        #endregion

        #region Private Methods

        /**
         * スクリーンセーバーを表示させる
         */
        private void ShowScreenSaver()
        {
            if (_isScreenSaverActive) return; // 既に表示されている場合は何もしない

            // スクリーンセーバーが動画の場合は０秒にシーク
            if (_hasVideoPlayer && _videoPlayer != null)
            {
                _videoPlayer.time = 0; // 動画の再生位置を0秒に設定
                _videoPlayer.Play(); // 動画を再生
            }

            // スクリーンセーバーのオブジェクトを表示
            screenSaverObj.SetActive(true);

            _isScreenSaverActive = true; // スクリーンセーバーが表示されている状態に更新
        }

        /**
         * スクリーンセーバーを非表示にする
         */
        private void HideScreenSaver(bool force = false)
        {
            // 既に非表示の場合は何もしない
            if (!_isScreenSaverActive && !force) return;

            // スクリーンセーバーのオブジェクトを非表示
            screenSaverObj.SetActive(false);

            // 動画プレイヤーがある場合は停止
            if (_hasVideoPlayer && _videoPlayer != null)
                _videoPlayer.Stop(); // 動画の再生を停止

            _isScreenSaverActive = false; // スクリーンセーバーが非表示の状態に更新
        }

        #endregion
    }
}