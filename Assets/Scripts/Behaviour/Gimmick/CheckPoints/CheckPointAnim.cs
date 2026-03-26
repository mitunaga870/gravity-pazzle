using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Behaviour.Gimmick.CheckPoints
{
    /// <summary>
    /// チェックポイントのアニメーションのみを管理するクラス
    /// </summary>
    [RequireComponent(typeof(CheckPoint))]
    public class CheckPointAnim : MonoBehaviour
    {
        [SerializeField]
        private Renderer colorBoxRenderer;

        [Header("アニメーション対象（上下・回転ともこの Transform）")]
        [SerializeField]
        private Transform rotationTarget;

        [Header("ふわふわ（上下）")]
        [SerializeField]
        private float floatAmplitude = 0.12f;

        [SerializeField]
        private float floatCycleDuration = 2.2f;

        [Header("回転")]
        [SerializeField]
        private float rotationCycleDuration = 9f;

        private static readonly Color ActiveColor = Color.green;
        private static readonly Color InactiveColor = Color.red;

        private Vector3 _baseLocalPosition;
        private Quaternion _baseLocalRotation;

        private void Awake()
        {
            if (rotationTarget != null)
                _baseLocalPosition = rotationTarget.localPosition;
        }

        private void Start()
        {
            // 初期状態は非アクティブ
            SetInactive();
            StartFloatAndRotationMotions();
        }

        private void StartFloatAndRotationMotions()
        {
            if (rotationTarget == null)
                return;

            _baseLocalRotation = rotationTarget.localRotation;

            // 上下にふわふわ（往復＝floatCycleDuration 秒）
            LMotion.Create(0f, floatAmplitude, floatCycleDuration * 0.5f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(yOffset =>
                {
                    var p = _baseLocalPosition;
                    p.y += yOffset;
                    rotationTarget.localPosition = p;
                })
                .AddTo(this);

            // ローカル Y 軸周り（オイラー補間では傾き時に軸がずれるためクォータニオンで積む）
            LMotion.Create(0f, 360f, rotationCycleDuration)
                .WithLoops(-1, LoopType.Restart)
                .Bind(angle =>
                {
                    rotationTarget.localRotation =
                        _baseLocalRotation * Quaternion.AngleAxis(angle, Vector3.up);
                })
                .AddTo(this);
        }
        
        public void SetActive()
        {
            if (colorBoxRenderer == null) return;
                
            // アクティブな状態を緑色で表現
            colorBoxRenderer.material.color = ActiveColor;
        }

        public void SetInactive()
        {
            if (colorBoxRenderer == null) return;

            // 非アクティブな状態を赤色で表現
            colorBoxRenderer.material.color = InactiveColor;
        }
    }
}