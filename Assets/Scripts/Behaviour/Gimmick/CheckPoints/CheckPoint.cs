using System;
using Behaviour.Controller.Stage;
using Behaviour.ObjectFeature;
using Lib.State.Interface.Gravity;
using UnityEngine;

namespace Behaviour.Gimmick.CheckPoints
{
    /// <summary>
    /// プレイヤーが近づいた時に、プレイヤーの初期位置をこのオブジェクトの位置に変更するオブジェクト
    /// コライダーはトリガーにすること
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField]
        private GravType gravTypeOnPoint;

        private StageDataController _stageDataController;
        private CheckPointAnim _checkPointAnim;
        
        private bool _isActive;
        public bool IsActive => _isActive;
        private void Start()
        {
            _stageDataController = StageDataController.Instance;
            _checkPointAnim = GetComponent<CheckPointAnim>();
            _isActive = false;
        }
        
        public void OnTriggerEnter(Collider other)
        {
            ActiveCheckPoint(other);
        }
        
        // チェックポイントアクティブ処理
        private void ActiveCheckPoint(Collider other)
        {
            // すでにアクティブなチェックポイントは無視する
            if (_isActive) return;
            _isActive = true;
            
            // プレイヤー以外のオブジェクトは無視する
            if (!other.CompareTag("Player")) return;
            var resetableObject = other.GetComponent<ResetableObject>();
            if (resetableObject == null) return;
            
            // プレイヤーの初期位置をこのオブジェクトの位置に変更する
            resetableObject.OverWriteInitialPosition(transform.position);
            resetableObject.OverWriteInitialGravType(gravTypeOnPoint);
            
            // コントローラーに通知
            _stageDataController.ActivateCheckPoint(this);
            
            // Animをアクティブにする
            if (_checkPointAnim != null) _checkPointAnim.SetActive();
        }

        public void InActiveCheckPoint()
        {
            // すでに非アクティブなチェックポイントは無視する
            if (!_isActive) return;
            _isActive = false;
            
            // Animを非アクティブにする
            if (_checkPointAnim != null) _checkPointAnim.SetInactive();
        }
    }
}