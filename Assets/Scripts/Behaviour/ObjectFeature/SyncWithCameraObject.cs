using Behaviour.Camera;
using UnityEngine;

namespace Behaviour.ObjectFeature
{
    /// <summary>
    /// 同期カメラのRotateAroundを呼び出す時に同時に呼び出すと動きを同期できるクラス
    /// </summary>
    public class SyncWithCameraObject : MonoBehaviour
    {
        [SerializeField]
        private PlayerCam playerCam;
        
        private void Update()
        {
            var objTrans = transform;
            
            // カメラの回転を取得
            var cameraRotation = playerCam.transform.rotation;
            // オブジェクトの回転をカメラの回転を反転して同期
            objTrans.rotation = Quaternion.Inverse(cameraRotation);
        }
    }
}