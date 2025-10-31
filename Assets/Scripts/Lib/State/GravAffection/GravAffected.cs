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
        }
        
        private readonly Vector3 _gravity;
        private readonly Rigidbody _affectedBody;
        private readonly Transform _focusCameraTransform;
        private readonly GravType _gravType;


        #region IGravAffectionState
        public GravAffectionState GetCurrentState => GravAffectionState.Affected;
        public GravType GravType => _gravType;

        public bool Adapting { get; private set; }
        
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

            // 適応中は変更不可
            if (Adapting && !forceChange)
                return false;
            
            if (next == null)
                return false;
            
            return true;
        }

        public void OnEnter(IGravAffectionState prev)
        {
            // カメラを指定位置が下になるように
            if (_hasCamera && prev != null && prev.GravType != _gravType)
            {
                Adapting = true;

                // 軸取得
                var (axis, angle) = GravUtils.GetGravToGravRotation(prev.GravType, _gravType);
                // axisがゼロベクトルの場合は１８０度回転
                if (axis == Vector3.zero)
                {
                    axis = _focusCameraTransform.right; // 適当な軸
                    angle = 180f;
                }

                var duration = 0.25f;

#if UNITY_EDITOR
                // 回転角を表示
                Debug.Log($"GravAffected OnEnter {_gravType}: Rotate Camera around {axis} by {angle} degrees.");
#endif

                LMotion.Create(0, angle, duration)
                    .WithEase(Ease.InOutSine)
                    .WithScheduler(MotionScheduler.PostLateUpdate)
                    .WithOnComplete(() => { Adapting = false; })
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
    }
}