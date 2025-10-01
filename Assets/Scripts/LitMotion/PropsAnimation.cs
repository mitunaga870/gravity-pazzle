using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Random = UnityEngine.Random;

public class PropsAnimation : MonoBehaviour
{
    [Header("Movement Parameters (移動のパラメータ)")]
    [Tooltip("XYZ軸方向へのランダムな移動の振幅。大きくすると大きく動きます。")]
    public Vector3 movementAmplitude = new Vector3(0.5f, 0.5f, 0.5f);
    
    [Tooltip("アニメーションの基本的な持続時間 (秒)")]
    public float baseDuration = 5f;
    
    [Tooltip("次のモーションに移行するまでのランダムな遅延の最大値 (秒)")]
    public float maxDelay = 1f;

    [Header("Rotation Parameters (回転のパラメータ)")]
    [Tooltip("XYZ軸回転の速さ（度/秒）")]
    public float rotationSpeed = 30f;

    // 個々のオブジェクトのランダムな動きを生成するためのシード
    private float randomSeed;
    // 初期の位置と回転を記憶（ローカル座標系）
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // オブジェクトごとに異なる動きにするためのランダムなシードを設定
        randomSeed = Random.value * 100f; 
        
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        StartMotion();
    }

    private void StartMotion()
    {
        float duration = baseDuration + Random.Range(-1f, 1f); 

        // --- 移動アニメーション（ふわふわ）のターゲット位置計算 ---
        
        float xNoiseCoord = (Time.time * 0.1f + randomSeed) * 0.5f;
        float yNoiseCoord = (Time.time * 0.1f + randomSeed + 10f) * 0.5f; 
        float zNoiseCoord = (Time.time * 0.1f + randomSeed + 20f) * 0.5f; 

        Vector3 randomOffset = new Vector3(
            (Mathf.PerlinNoise(xNoiseCoord, 0f) - 0.5f) * 2f * movementAmplitude.x,
            (Mathf.PerlinNoise(yNoiseCoord, 0f) - 0.5f) * 2f * movementAmplitude.y,
            (Mathf.PerlinNoise(zNoiseCoord, 0f) - 0.5f) * 2f * movementAmplitude.z
        );

        Vector3 endPosition = initialPosition + randomOffset;
        
        // --- 移動モーションの作成と実行 ---
        var moveHandle = LMotion.Create(transform.localPosition, endPosition, duration)
            .WithEase(Ease.InOutSine)
            
            .WithOnComplete(() =>
            {
                // 移動が完了したら、ランダムな遅延モーションを作成
                float delay = Random.Range(0f, maxDelay);
                
                // 遅延モーションを作成し、実行してMotionHandleを取得
                var delayHandle = LMotion.Create(0f, 1f, delay) 
                    .WithOnComplete(StartMotion)
                    .RunWithoutBinding(); // Bindしない場合はRunWithoutBinding()で実行

                // MotionHandleに対してAddToを呼び出す
                delayHandle.AddTo(gameObject);
            })
            
            .BindToLocalPosition(transform); // BindTo... の戻り値は MotionHandle

        // MotionHandleに対してAddToを呼び出す
        moveHandle.AddTo(gameObject);

        // --- 回転アニメーション（無限ループ） ---

        Vector3 randomRotationAxis = new Vector3(
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f)
        ).normalized;
        
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(randomRotationAxis * 360f);
        
        // MotionHandleを取得
        var rotationHandle = LMotion.Create(startRot, endRot, 360f / rotationSpeed) 
            .WithLoops(-1, LoopType.Restart)
            .WithEase(Ease.Linear)
            .BindToLocalRotation(transform);
        
        // MotionHandleに対してAddToを呼び出す
        rotationHandle.AddTo(gameObject);
    }
}