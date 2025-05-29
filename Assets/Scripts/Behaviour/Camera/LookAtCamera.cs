using UnityEngine;
//Sirofyuu 2025年5月27日


public class LookAtCamera : MonoBehaviour
{
    [Tooltip("このオブジェクトが常に見つめるカメラを指定します。未指定の場合はメインカメラが使用されます。")]
    public Camera targetCamera;

    [Tooltip("カメラの方を向かせたいオブジェクト（板ポリなど）を指定します。未指定の場合は、このスクリプトがアタッチされているオブジェクト自身が対象になります。")]
    public Transform objectToRotate;

    void Start()
    {
        // targetCameraがインスペクタで指定されていない場合、メインカメラを探して設定
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogError("LookAtCamera: ターゲットカメラが指定されておらず、メインカメラも見つかりませんでした。インスペクタでtargetCameraを指定してください。");
                enabled = false; // スクリプトを無効化
                return;
            }
        }

        // objectToRotateがインスペクタで指定されていない場合、このスクリプトがアタッチされているオブジェクトのTransformを使用
        if (objectToRotate == null)
        {
            objectToRotate = this.transform;
            Debug.LogWarning("LookAtCamera: objectToRotateが指定されていません。このゲームオブジェクトのTransform (" + gameObject.name + ") を使用します。");
        }
    }

    // Updateの代わりにLateUpdateを使用することを推奨します。
    // カメラの動きがUpdateで処理された後にオブジェクトの向きを調整するため、カクつきを防ぐことができます。
    void LateUpdate()
    {
        if (targetCamera != null && objectToRotate != null)
        {
            // オブジェクトのZ軸がカメラの方向を向くようにします。
            // これにより、オブジェクトの「正面」がカメラに向きます。
            objectToRotate.LookAt(targetCamera.transform);

            // もし板ポリの「裏面」がカメラに向いてしまう場合：
            // Unityの標準Quadなどは、Z軸のプラス側が「表」とみなされることが多いです。
            // LookAtはそのオブジェクトのZ軸プラス方向をターゲットに向けます。
            // もしQuadの「見える面」が逆（例えばモデリングソフトからのインポート時など）で、
            // LookAtを使うと裏返ってしまう場合は、以下のいずれかの方法を試せます。
            // 1. Quad自体を180度Y軸回転させておく。
            // 2. LookAtの後に回転を追加する:
            //    objectToRotate.LookAt(targetCamera.transform);
            //    objectToRotate.Rotate(0, 180, 0, Space.Self); // Y軸を中心に180度回転
            // 3. LookAtのターゲットを調整する (少し複雑になります):
            //    Vector3 directionFromCamera = objectToRotate.position - targetCamera.transform.position;
            //    objectToRotate.rotation = Quaternion.LookRotation(directionFromCamera);

            // 最も一般的なビルボード（常にカメラの平面に平行）にしたい場合：
            // objectToRotate.rotation = targetCamera.transform.rotation;
            // これだとオブジェクトはカメラと全く同じ向きになるので、
            // 板ポリの表面をカメラに見せるには、板ポリが元々X-Y平面にあり、
            // その法線がローカルZ軸方向を向いている必要があります。
        }
    }
}