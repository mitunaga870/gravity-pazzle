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
            _material.SetFloat(Radius, 0.2f); 
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