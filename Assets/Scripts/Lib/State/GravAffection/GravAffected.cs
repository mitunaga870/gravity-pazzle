#region

using System;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using UnityEditor.Search;
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
            // カメラを指定位置が下になるように
            if (_hasCamera && prev != null)
            {
                var (axis, angle) = GravUtils.GetGravToGravRotation(prev.GravType, _gravType);
                
                // axisがゼロベクトルの場合は１８０度回転
                if (axis == Vector3.zero)
                {
                    axis = _focusCameraTransform.right; // 適当な軸
                    angle = 180f;
                    
                    // ただ１８０度回すと床に埋まるのでプレイヤーの高さ分上げる
                    // TODO: 環境設定によって変える（設定のPRマージ後）
                    const float heightOffset = 1.0f;
                    var topVector = GravUtils.GetGravDirectionUnit(
                        GravUtils.GetUpperGravType(prev.GravType));
                    
                    var offset = topVector * heightOffset;
                }

                _affectedBody.transform.RotateAround(
                    _affectedBody.position,
                    axis,
                    angle);

                _focusCameraTransform.RotateAround(
                    _affectedBody.position,
                    axis,
                    angle);
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