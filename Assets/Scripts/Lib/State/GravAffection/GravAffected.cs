#region

using System;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using LitMotion;
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
            
            InitCameraPos();
        }
        
        private readonly Vector3 _gravity;
        private readonly Rigidbody _affectedBody;
        private readonly Transform _focusCameraTransform;
        private readonly GravType _gravType;

        // カメラとプレイヤーの距離
        private float _cameraDistance;
        
        #region IGravAffectionState
        public GravAffectionState GetCurrentState => GravAffectionState.Affected;
        public GravType GravType => _gravType;
        private readonly bool _hasCamera;

        private float _curAngle;

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
                }

                var duration = 0.25f;

                LMotion.Create(0, angle, duration)
                    .WithEase(Ease.InOutSine)
                    .WithScheduler(MotionScheduler.PostLateUpdate)
                    .Bind(this, (nextAngle, self) =>
                    {
                        var diff = nextAngle - self._curAngle;
                        self._curAngle = nextAngle;
                        
                        var parent = self._focusCameraTransform.parent;
                        
                        parent.RotateAround(
                                _affectedBody.position,
                            axis,
                            diff);
                    });
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

        #region private methods

        /// <summary>
        /// カメラとプレイヤーの位置関係を初期化
        /// </summary>
        private void InitCameraPos()
        {
            if (!_hasCamera)
                return;

            _cameraDistance = Vector3.Distance(_focusCameraTransform.position, _affectedBody.position);
        }
        #endregion
    }
}