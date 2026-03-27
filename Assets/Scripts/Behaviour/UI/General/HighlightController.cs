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
        [SerializeField, Range(0.05f, 1f)] private float maxHighlightRadius = 0.45f;
        
        // 反転マスクオブジェクト
        private Image _maskImage;
        private Material _material;
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Center = Shader.PropertyToID("_Center");
        
        // カメラ
        private UnityEngine.Camera _mainCamera;
        
        // highlightさせたいオブジェクト
        private GameObject _target;

        private void SetMaskVisible(bool visible)
        {
            if (_maskImage != null && _maskImage.enabled != visible)
                _maskImage.enabled = visible;
        }

        /// <summary>
        /// シェーダー用の中心（UV）と半径。中心はバウンディングの幾何中心（ピボットではない）に揃え、円の半径はその中心から角までの距離の最大値とする。
        /// </summary>
        private bool TryGetHighlightCircle(GameObject target, out Vector2 uvCenter, out float radius)
        {
            uvCenter = default;
            radius = 0.2f;
            if (target == null || _mainCamera == null)
                return false;

            // UI は子に Renderer 等が付いていても矩形を正とする（子の巨大 bounds に引っ張られない）
            var rect = target.GetComponent<RectTransform>();
            if (rect != null)
            {
                // 中心をワールド座標に変換
                var centerWorld = rect.TransformPoint(rect.rect.center);
                var screen = WorldToScreenPointForTarget(centerWorld);
                if (screen.z < 0f)
                    return false;
                uvCenter = new Vector2(screen.x / Screen.width, screen.y / Screen.height);
                
                // シェーダー用の中心（UV）を計算
                var aspect = (float)Screen.width / Screen.height;
                var centerShader = new Vector2(uvCenter.x * aspect, uvCenter.y);
                
                // 角から最大距離を計算
                var corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                radius = MaxDistanceFromScreenCorners(corners, centerShader, WorldToScreenPointForTarget);

                return true;
            }

            // レンダラーから最大距離を計算
            var renderer = target.GetComponentInChildren<Renderer>();
            if (renderer != null)
                return TryGetHighlightCircleFromBounds(renderer.bounds, out uvCenter, out radius);

            // コライダーから最大距離を計算
            var collider = target.GetComponentInChildren<Collider>();
            if (collider != null)
                return TryGetHighlightCircleFromBounds(collider.bounds, out uvCenter, out radius);

            var screenPivot = WorldToScreenPointForTarget(target.transform.position);
            if (screenPivot.z < 0f)
                return false;
            uvCenter = new Vector2(screenPivot.x / Screen.width, screenPivot.y / Screen.height);
            radius = 0.2f;
            return true;
        }

        private bool TryGetHighlightCircleFromBounds(Bounds bounds, out Vector2 uvCenter, out float radius)
        {
            uvCenter = default;
            radius = 0.2f;
            var aspect = (float)Screen.width / Screen.height;

            // 中心をスクリーン座標に変換
            var screen = WorldToScreenPointForTarget(bounds.center);
            if (screen.z < 0f)
                return false;
            uvCenter = new Vector2(screen.x / Screen.width, screen.y / Screen.height);
            var centerShader = new Vector2(uvCenter.x * aspect, uvCenter.y);

            // 角から最大距離を計算
            var c = bounds.center;
            var e = bounds.extents;
            var corners = new Vector3[8];
            var i = 0;
            for (var ix = -1; ix <= 1; ix += 2)
            for (var iy = -1; iy <= 1; iy += 2)
            for (var iz = -1; iz <= 1; iz += 2)
                corners[i++] = c + new Vector3(ix * e.x, iy * e.y, iz * e.z);

            // 角から最大距離を計算
            radius = MaxDistanceFromScreenCorners(corners, centerShader, WorldToScreenPointForTarget);
            radius = Mathf.Min(radius, maxHighlightRadius);
            return true;
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
                // 画面外へ飛んだ投影点をそのまま使うと半径が暴走するため、画面端でクランプする
                var u = Mathf.Clamp01(sp.x / Screen.width);
                var v = Mathf.Clamp01(sp.y / Screen.height);
                var p = new Vector2(u * aspect, v);
                var d = Vector2.Distance(p, centerShader);
                if (d > maxDist)
                    maxDist = d;
            }

            // 最大距離を返す
            var fallback = Mathf.Min(0.2f, maxHighlightRadius);
            return maxDist > 0f ? Mathf.Min(maxDist, maxHighlightRadius) : fallback;
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
            _maskImage = GetComponent<Image>();
            _material = new Material(_maskImage.material);
            _maskImage.material = _material;
            SetMaskVisible(false);
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

            Vector2 uvPos;
            float radius;

            if (isMouseFollowMode && Debug.isDebugBuild)
            {
                var targetPos = Input.mousePosition;
                uvPos = new Vector2(targetPos.x / Screen.width, targetPos.y / Screen.height);
                radius = Mathf.Min(0.2f, maxHighlightRadius);
                SetMaskVisible(true);
            }
            else
            {
                if (_target == null)
                {
                    SetMaskVisible(false);
                    return;
                }
                if (!TryGetHighlightCircle(_target, out uvPos, out radius))
                {
                    SetMaskVisible(true);
                    _material.SetFloat(Radius, 0f);
                    return;
                }
                SetMaskVisible(true);
            }

            var targetPixel = new Vector2(uvPos.x * Screen.width, uvPos.y * Screen.height);
            var outOfCamera =
                targetPixel.x < 0 ||
                targetPixel.x > Screen.width ||
                targetPixel.y < 0 ||
                targetPixel.y > Screen.height;

            // カメラの外なら反転マスクをなし
            if (outOfCamera)
            {
                SetMaskVisible(true);
                _material.SetFloat(Radius, 0f);
                return;
            }

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