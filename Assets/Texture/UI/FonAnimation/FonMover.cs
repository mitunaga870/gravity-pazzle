using UnityEngine;
using LitMotion;


public class FonMover : MonoBehaviour
{
 [Header("対象RectTransform")]
    public RectTransform target;

    [Header("開始（A）状態")]
    public Vector3 startPosition;
    public Vector3 startRotation;
    public Vector3 startScale = Vector3.one;

    [Header("終了（B）状態")]
    public Vector3 endPosition;
    public Vector3 endRotation;
    public Vector3 endScale = Vector3.one;

    [Header("補間時間（秒）")]
    public float duration = 1.0f;

    [Header("Ease設定")]
    public Ease ease = Ease.InOutQuad;

    [Header("逆方向に再生する（B→A）")]
    public bool reverse = false;

    private MotionHandle positionHandle;
    private MotionHandle rotationHandle;
    private MotionHandle scaleHandle;

    [ContextMenu("Play Motion")]
    public void PlayMotion()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        // A→B か B→A かをチェック
        Vector3 fromPos = reverse ? endPosition : startPosition;
        Vector3 toPos   = reverse ? startPosition : endPosition;

        Vector3 fromRot = reverse ? endRotation : startRotation;
        Vector3 toRot   = reverse ? startRotation : endRotation;

        Vector3 fromScale = reverse ? endScale : startScale;
        Vector3 toScale   = reverse ? startScale : endScale;

        // 初期化
        target.anchoredPosition3D = fromPos;
        target.localEulerAngles = fromRot;
        target.localScale = fromScale;

        // 位置補間
        positionHandle = LMotion.Create(fromPos, toPos, duration)
            .WithEase(ease)
            .Bind(value => target.anchoredPosition3D = value);

        // 回転補間
        rotationHandle = LMotion.Create(fromRot, toRot, duration)
            .WithEase(ease)
            .Bind(value => target.localEulerAngles = value);

        // スケール補間
        scaleHandle = LMotion.Create(fromScale, toScale, duration)
            .WithEase(ease)
            .Bind(value => target.localScale = value);
    }
}
