using UnityEngine;

namespace Behaviour.ObjectFeature.RideableObjectBehaviours
{
    public class RiderObject: MonoBehaviour
    {
        
        [SerializeField]
        private Rigidbody rb;

        public GameObject RidingObject
        {
            get => ridingObject;
            set
            {
                // 乗っているオブジェクトがnullの場合、乗っていないとみなしそうでなければ乗る
                if (value == null)
                {
                    isRiding = false;
                    ridingObject = null;
                }
                else
                {
                    isRiding = true;
                    ridingObject = value;
                    
                    // 初期位置登録
                    prevPosition = ridingObject.transform.position;
                }
            }
        }
        private GameObject ridingObject;
        private bool isRiding;
        
        private Vector3 prevPosition;
        
        private void FixedUpdate()
        {
            if (!isRiding) return;
            
            var ridingPos = ridingObject.transform.position;
            
            // 乗っているオブジェクトの位置変異に合わせて移動させる
            var delta = ridingPos - prevPosition;
            rb.MovePosition(rb.position + delta);

            // 直前の位置を更新
            prevPosition = ridingPos;
        }
    }
}