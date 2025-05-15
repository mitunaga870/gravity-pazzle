using System;
using Behaviour.Gravity;
using Behaviour.Gravity.Abstract;
using Lib.Logic;
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
            // inActiveならアクティブにする
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            
            // 速度をリセット
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Rigidbodyがある場合
                // 一時的に位置固定
                rb.isKinematic = true;
                // 速度をリセット
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                
                // RigidbodyのisKinematicをfalseに戻す
                var delay = GeneralUtils.DelayCoroutine(
                    0.1f,
                    () =>
                    {
                        rb.isKinematic = false;
                    });
                StartCoroutine(delay);
            }
            
            // 重力を戻す
            var grav = GetComponent<VGravBehaviour>();
            if (grav != null)
            {
                // GravBehaviourがある場合
                // 重力を初期値に戻す
                grav.SetGravAffected(grav.InitialGravType);
            }
            
            // 初期位置に戻す
            transform.position = _initialPosition;
        }
    }
}