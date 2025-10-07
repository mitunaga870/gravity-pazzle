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
                
                // カメラを重力方向に合わせて回転
                if (_hasCamera && _focusCameraTransform != null)
                {
                    var cameraRot = _focusCameraTransform.rotation;
                    LMotion.Create(cameraRot, cameraRot * gravRot, moveTime)
                        .BindToRotation(_focusCameraTransform);
                }
                
                // プレイヤーも重力方向に合わせて回転
                if (_affectedBody != null)
                {
                    var playerRot = _affectedBody.transform.rotation;
                    LMotion.Create(playerRot, playerRot * gravRot, moveTime)
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