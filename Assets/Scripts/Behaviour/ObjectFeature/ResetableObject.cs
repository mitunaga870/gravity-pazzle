using System;
using UnityEngine;

namespace Behaviour.ObjectFeature
{
    /// <summary>
    /// 初期位置を覚え、初期位置に戻す機能を持つオブジェクト
    /// </summary>
    public class ResetableObject : MonoBehaviour
    {
        private Vector3 _initialPosition;
        private void Start()
        {
            // 初期位置を覚える
            _initialPosition = transform.position;
        }
        
        [Obsolete("Obsolete")]
        public void ResetPosition()
        {
            // 初期位置に戻す
            transform.position = _initialPosition;
            
            // 速度をリセット
            var rb = GetComponent<Rigidbody>();
            if (rb == null) return;
            
            // Rigidbodyがある場合、速度をリセット
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}