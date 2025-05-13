using Behaviour.Gravity.Abstract;
using Lib.Logic;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Lib.State.GravAffection
{
    public class GravAffected: IGravAffectionState
    {
        public GravAffected(
            GravType gravType,
            Rigidbody affectedBody,
            Transform focusCameraTransform = null)
        {
            _affectedBody = affectedBody;
            _gravType = gravType;
            _gravity = GravUtils.GetGravAcceleration(gravType);
            _focusCameraTransform = focusCameraTransform;
            
            // カメラが指定されているか
            _hasCamera = focusCameraTransform != null;
        }
        
        private readonly Vector3 _gravity;
        private readonly Rigidbody _affectedBody;
        private readonly Transform _focusCameraTransform;
        private readonly GravType _gravType;
        
        #region IGravAffectionState
        public GravAffectionState GetCurrentState => GravAffectionState.Affected;
        public GravType GravType => _gravType;
        private readonly bool _hasCamera;

        public bool Change(IGravAffectionState next)
        {
            if (next == null)
                return false;
            
            
            return true;
        }

        public void OnEnter(IGravAffectionState prev)
        {
            // カメラを指定位置が下になるように
            if (_hasCamera && prev != null)
            {
                // カメラ移動時間
                var moveTime = 0.5f;
                
                // 既存カメラの向き
                var cameraRot = _focusCameraTransform.rotation;
                
                // 重力の向きベクトル
                var prevGrav = GravUtils.GetGravDirectionUnit(prev.GravType);
                var currGrav = _gravity.normalized;
                
                // 重力がどう回転したか計算
                var gravRot = Quaternion.FromToRotation(prevGrav, currGrav);
                
                // カメラの向きを重力の向きに合わせる
                LMotion.Create(cameraRot,   cameraRot * gravRot, moveTime)
                    .BindToRotation(_affectedBody.transform);
            }
        }

        public void OnExit()
        {
            
        }

        public void OnFixedUpdate()
        {
            if (_affectedBody == null)
                return;
            
            // 重力の影響を受ける
            _affectedBody.AddForce(_gravity, ForceMode.Acceleration);
        }
        #endregion
    }
}