using Behaviour.Camera;
using UnityEngine;

namespace Behaviour.ObjectFeature
{
    public class SyncWithCameraObject : MonoBehaviour
    {
        [SerializeField]
        private PlayerCam playerCam;
        
        private void Update()
        {
            transform.rotation = playerCam.transform.rotation;
        }
    }
}