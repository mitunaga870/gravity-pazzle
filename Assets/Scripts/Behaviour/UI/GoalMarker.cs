using UnityEngine;
using TMPro;  // TextMeshProUGUI を使う

namespace Behaviour.UI
{
    /// <summary>
    /// ゴール位置を示すマーカーUI。
    /// 画面内ならワールド位置に重ね、画面外なら端に固定して回転で方向を示す。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class GoalMarker : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;              // ゴールのTransform

        [SerializeField]
        private UnityEngine.Camera _playerCamera; // プレイヤー視点のカメラ（未設定時は MainCamera を自動取得）

        [SerializeField]
        private float _screenEdgeBuffer = 20f;  // 画面端からの余白（ピクセル）

        [SerializeField]
        private GameObject _distanceTextObject; // 子オブジェクト "DistanceText" の GameObject

        private TextMeshProUGUI _distanceText;                     // TextMeshProUGUI に変更

        private RectTransform _markerRect;      
        private RectTransform _canvasRect;      

        [SerializeField, Range(90f, 180f)]
        private float _backAngleThreshold = 120f;  // 背面判定に使う角度閾値(°)

        private void Awake()
        {
            _markerRect = GetComponent<RectTransform>();
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                _canvasRect = canvas.GetComponent<RectTransform>();

            if (_playerCamera == null)
                _playerCamera = UnityEngine.Camera.main;

            // 子オブジェクトから TextMeshProUGUI を取得
            if (_distanceTextObject != null)
                _distanceText = _distanceTextObject.GetComponent<TextMeshProUGUI>();
        }

        private int _updateFrameInterval = 5; // Update every 5 frames
        private int _frameCounter = 0;        // Frame counter for throttling updates

        private void Update()
        {
            if (_target == null || _playerCamera == null || _canvasRect == null)
                return;

            // Skip updates until the frame interval is reached
            _frameCounter++;
            if (_frameCounter < _updateFrameInterval)
                return;

            _frameCounter = 0; // Reset the counter
            // ワールド→ビューポート
            Vector3 vpPos = _playerCamera.WorldToViewportPoint(_target.position);
            // カメラ→ゴール方向ベクトル
            Vector3 dir = (_target.position - _playerCamera.transform.position).normalized;
            // カメラ前方向とのなす角 (0°:前,180°:後ろ)
            float viewAngle = Vector3.Angle(_playerCamera.transform.forward, dir);

            // ビューポート z<0 だけでなく、なす角が閾値より大きい場合を背面扱い
            bool isBehind = viewAngle > _backAngleThreshold;
            bool isOffScreen = isBehind
                || vpPos.x < 0 || vpPos.x > 1
                || vpPos.y < 0 || vpPos.y > 1;

            // z<0 の反転処理は残しておいてOK
            if (vpPos.z < 0)
            {
                vpPos.x = 1f - vpPos.x;
                vpPos.y = 1f - vpPos.y;
            }

            // ビューポートを0～1にクランプ
            float x = Mathf.Clamp01(vpPos.x);
            float y = Mathf.Clamp01(vpPos.y);
            Vector2 anchored = new Vector2(
                x * _canvasRect.sizeDelta.x - _canvasRect.sizeDelta.x * 0.5f,
                y * _canvasRect.sizeDelta.y - _canvasRect.sizeDelta.y * 0.5f
            );

            // 端からの余白を考慮してクランプ
            anchored.x = Mathf.Clamp(
                anchored.x,
                -_canvasRect.sizeDelta.x * 0.5f + _screenEdgeBuffer,
                _canvasRect.sizeDelta.x * 0.5f - _screenEdgeBuffer
            );
            anchored.y = Mathf.Clamp(
                anchored.y,
                -_canvasRect.sizeDelta.y * 0.5f + _screenEdgeBuffer,
                _canvasRect.sizeDelta.y * 0.5f - _screenEdgeBuffer
            );

            // カメラ背面なら必ず画面下部に配置
            if (isBehind)
            {
                anchored.y = -_canvasRect.sizeDelta.y * 0.5f + _screenEdgeBuffer;
            }

            // マーカー配置
            _markerRect.anchoredPosition = anchored;

            // 画面外なら矢印回転、画面内なら無回転
            if (isOffScreen)
            {
                // Atan2で計算した角度に+90°オフセットを加え、
                // デフォルトで「下向き」を向いている矢印を目的方向に合わせる
                float angle = Mathf.Atan2(vpPos.y - 0.5f, vpPos.x - 0.5f) * Mathf.Rad2Deg + 90f;
                _markerRect.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                _markerRect.rotation = Quaternion.identity;
            }

            // 距離を小数１位まで取得して表示
            if (_distanceText != null)
            {
                float distance = Vector3.Distance(
                    _playerCamera.transform.position,
                    _target.position
                );
                _distanceText.text = $"{distance:F1}m";  // TextMeshProと同じプロパティ名
            }
        }
    }
}
