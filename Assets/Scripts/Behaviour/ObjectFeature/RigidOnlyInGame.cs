using Behaviour.Controller.General;
using Lib.State.Scene;
using UnityEngine;

namespace Behaviour.ObjectFeature
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidOnlyInGame : MonoBehaviour
    {
        
        private Rigidbody _rigidbody;
        
        private Vector3 _prevVelocity;
        
        private Vector3 _prevAngularVelocity;
        
        #region Unity Methods
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            // シーン状態コントローラーを取得
            var sceneStateController = SceneStateController.Instance;
            if (sceneStateController == null)
                Debug.LogError("SceneStateControllerが見つかりませんでした。");
            
            // シーン状態の変更イベントにリスナーを登録
            sceneStateController.AddOnSceneStateChanged(SceneState.InGame, OnChangeIngame);
            sceneStateController.AddOnSceneStateChanged(SceneState.Pause, OnChangePause);
        }
        
        #endregion

        private void OnChangeIngame()
        {
            // 物理挙動を再開
            _rigidbody.isKinematic = false;
            
            // 前の速度を復元
            _rigidbody.linearVelocity = _prevVelocity;
            _rigidbody.angularVelocity = _prevAngularVelocity;
        }
        
        private void OnChangePause()
        {
            // 現在の速度を保存
            _prevVelocity = _rigidbody.linearVelocity;
            _prevAngularVelocity = _rigidbody.angularVelocity;
            
            // 速度を0にして物理挙動を停止
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            
            // 物理挙動を停止
            _rigidbody.isKinematic = true;
        }
    }
}