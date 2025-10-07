#region

using System;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

#endregion

namespace Lib.State.GravAffection
{
    /// <summary>
    ///     重力の状態を受けているステート
    /// </summary>
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

        [Obsolete("Obsolete")]
        public bool Change(IGravAffectionState next, bool forceChange = false) 
        {
            // 速度がゼロでない場合は変更不可
            if (_affectedBody == null &&
                (_affectedBody.velocity.sqrMagnitude > 0.01f || !forceChange) // 速度が０か強制フラグ
               )
                return false;
            
            if (next == null)
                return false;
            
            return true;
        }

        public void OnEnter(IGravAffectionState prev)
        {
            if (prev != null)
            {
                // アニメーション時間
                var moveTime = 0.5f;
                
                // 重力の向きベクトル（前の重力方向と現在の重力方向）
                var prevGrav = GravUtils.GetGravDirectionUnit(prev.GravType);
                var currGrav = _gravity.normalized;
                
                // 重力がどう回転したか計算
                var gravRot = Quaternion.FromToRotation(prevGrav, currGrav);
                
                // カメラを重力方向に合わせて正しい向きで回転
                if (_hasCamera && _focusCameraTransform != null)
                {
                    // カメラの上方向も重力の反対方向に設定
                    var cameraTargetUp = -currGrav;
                    var currentCameraForward = _focusCameraTransform.forward;
                    
                    // カメラの前方向を新しい上方向に垂直な平面に投影
                    var cameraTargetForward = Vector3.ProjectOnPlane(currentCameraForward, cameraTargetUp).normalized;
                    if (cameraTargetForward == Vector3.zero)
                    {
                        // 前方向が重力方向と平行の場合、適当な方向を選択
                        cameraTargetForward = Vector3.ProjectOnPlane(Vector3.forward, cameraTargetUp).normalized;
                        if (cameraTargetForward == Vector3.zero)
                            cameraTargetForward = Vector3.ProjectOnPlane(Vector3.right, cameraTargetUp).normalized;
                    }
                    
                    // カメラの目標回転を計算
                    var cameraTargetRotation = Quaternion.LookRotation(cameraTargetForward, cameraTargetUp);
                    
                    LMotion.Create(_focusCameraTransform.rotation, cameraTargetRotation, moveTime)
                        .BindToRotation(_focusCameraTransform);
                }
                
                // プレイヤーを重力方向に合わせて正しい向きで回転
                if (_affectedBody != null)
                {
                    // プレイヤーの足元が新しい重力方向の反対を向くように回転を計算
                    var targetUp = -currGrav; // 重力の反対方向がプレイヤーの上方向
                    var currentForward = _affectedBody.transform.forward;
                    
                    // 新しい前方向を計算（上方向に垂直で、できるだけ現在の前方向を維持）
                    var targetForward = Vector3.ProjectOnPlane(currentForward, targetUp).normalized;
                    if (targetForward == Vector3.zero)
                    {
                        // 前方向が重力方向と平行の場合、適当な方向を選択
                        targetForward = Vector3.ProjectOnPlane(Vector3.forward, targetUp).normalized;
                        if (targetForward == Vector3.zero)
                            targetForward = Vector3.ProjectOnPlane(Vector3.right, targetUp).normalized;
                    }
                    
                    // 目標回転を計算
                    var targetRotation = Quaternion.LookRotation(targetForward, targetUp);
                    
                    LMotion.Create(_affectedBody.transform.rotation, targetRotation, moveTime)
                        .BindToRotation(_affectedBody.transform);
                }
                
                Debug.Log($"Player & Camera rotation: {prev.GravType} → {_gravType}, rotation: {gravRot.eulerAngles}");
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