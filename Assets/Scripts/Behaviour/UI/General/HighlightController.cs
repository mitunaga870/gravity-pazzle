using UnityEngine;
using UnityEngine.UI;

namespace Behaviour.UI.General
{
    /// <summary>
    /// チュートリアルなどに使うhighlight（指定オブジェクト・UIの周辺以外を暗くするやつ）のコントローラー
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class HighlightController : MonoBehaviour
    {
        public static HighlightController Instance { get; private set; }

        // デバッグ用マウス追従モードフラグ
        [SerializeField] private bool isMouseFollowMode;
        
        // 反転マスクオブジェクト
        private Material _material;
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Center = Shader.PropertyToID("_Center");
        
        // カメラ
        private UnityEngine.Camera _mainCamera;
        
        // highlightさせたいオブジェクト
        private GameObject _target;

        /// <summary>
        /// Highlight.shader の距離計算と同じ座標系（x に画面アスペクトを反映）で、中心からバウンディングまでの最大距離を返す。
        /// </summary>
        private float ComputeHighlightRadius(GameObject target, Vector2 centerUv)
        {
            if (target == null || _mainCamera == null)
                return 0.2f;

            var aspect = (float)Screen.width / Screen.height;
            var centerShader = new Vector2(centerUv.x * aspect, centerUv.y);

            // レンダラーから最大距離を計算
            var renderer = target.GetComponentInChildren<Renderer>();
            if (renderer != null)
                return MaxDistanceFromBoundsCorners(renderer.bounds, centerShader, WorldToScreenPointForTarget);

            // コライダーから最大距離を計算
            var collider = target.GetComponentInChildren<Collider>();
            if (collider != null)
                return MaxDistanceFromBoundsCorners(collider.bounds, centerShader, WorldToScreenPointForTarget);

            // UIから最大距離を計算
            var rect = target.GetComponent<RectTransform>();
            if (rect != null)
            {
                var corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                return MaxDistanceFromScreenCorners(corners, centerShader, WorldToScreenPointForTarget);
            }

            // デフォルト値
            return 0.2f;
        }

        /// <summary>
        /// 対象のワールド座標をスクリーン座標に変換する
        /// </summary>
        private Vector3 WorldToScreenPointForTarget(Vector3 worldPoint)
        {
            // 対象がUIの場合
            var canvas = _target != null ? _target.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return RectTransformUtility.WorldToScreenPoint(null, worldPoint);
            
            // 対象がオブジェクトの場合
            var cam = canvas != null && canvas.worldCamera != null ? canvas.worldCamera : _mainCamera;
            return cam != null ? cam.WorldToScreenPoint(worldPoint) : RectTransformUtility.WorldToScreenPoint(null, worldPoint);
        }

        /// <summary>
        /// 対象のバウンディングボックスの角から最大距離を計算する
        /// </summary>
        private float MaxDistanceFromBoundsCorners(Bounds bounds, Vector2 centerShader, System.Func<Vector3, Vector3> worldToScreen)
        {
            // 対象のバウンディングボックスの中心とサイズを取得
            var center = bounds.center;
            var extents = bounds.extents;
            
            // 対象のバウンディングボックスの角を取得
            var corners = new Vector3[8];
            var i = 0;
            for (var ix = -1; ix <= 1; ix += 2)
            for (var iy = -1; iy <= 1; iy += 2)
            for (var iz = -1; iz <= 1; iz += 2)
                corners[i++] = center + new Vector3(ix * extents.x, iy * extents.y, iz * extents.z);

            // 対象のバウンディングボックスの角から最大距離を計算
            return MaxDistanceFromScreenCorners(corners, centerShader, worldToScreen);
        }

        /// <summary>
        /// 対象のスクリーン座標の角から最大距離を計算する
        /// </summary>
        private float MaxDistanceFromScreenCorners(Vector3[] worldCorners, Vector2 centerShader, System.Func<Vector3, Vector3> worldToScreen)
        {
            // 対象のスクリーン座標の角から最大距離を計算
            var aspect = (float)Screen.width / Screen.height;
            var maxDist = 0f;
            
            // 対象のスクリーン座標の角から最大距離を計算
            foreach (var corner in worldCorners)
            {
                var sp = worldToScreen(corner);
                if (sp.z < 0f)
                    continue;
                var u = sp.x / Screen.width;
                var v = sp.y / Screen.height;
                var p = new Vector2(u * aspect, v);
                var d = Vector2.Distance(p, centerShader);
                if (d > maxDist)
                    maxDist = d;
            }

            // 最大距離を返す
            return maxDist > 0f ? maxDist : 0.2f;
        }

        #region Unity Method

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // UI の Graphic は Renderer と違い material が共有アセットのまま返る。
            // SetVector 等で直接書き換えると .mat アセットが汚れるのでランタイム用に複製する。
            // Start より前に用意しておく（OnEnable 等で SetHighlight された直後の Update でも反映できるようにする）。
            var image = GetComponent<Image>();
            _material = new Material(image.material);
            image.material = _material;
        }

        private void Start()
        {
            _mainCamera = UnityEngine.Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("Main Camera is not found.");
                
                gameObject.SetActive(false);
                return;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (_material != null)
                Destroy(_material);
        }

        private void LateUpdate()
        {
            if (_material == null)
                return;

            Vector2 targetPos;
            // デバッグ関連処理
            if (isMouseFollowMode && Debug.isDebugBuild)
            {
                targetPos = Input.mousePosition;
            }
            else
            {
                // 対象オブジェクトの位置を取得
                if (_target == null) return;
                targetPos = _mainCamera.WorldToScreenPoint(_target.transform.position);
            }
            
            // カメラの外であるかどうかを判定
            var outOfCamera = 
                targetPos.x < 0 ||
                targetPos.x > Screen.width ||
                targetPos.y < 0 ||
                targetPos.y > Screen.height;

            // カメラの外なら反転マスクをなし
            if (outOfCamera) return;
            
            // カメラの内なら反転マスクをターゲットの位置に移動(uv座標系で)
            var uvPos = new Vector2(targetPos.x / Screen.width, targetPos.y / Screen.height);
            var radius = ComputeHighlightRadius(_target, uvPos);
            _material.SetFloat(Radius, radius);
            _material.SetVector(Center, uvPos);
        }

        #endregion

        #region Public Method

        public void SetHighlight(GameObject highlightTarget)
        {
            _target = highlightTarget;
        }

        #endregion
    }
}