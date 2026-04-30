#region

using System;
using System.Threading.Tasks;
using Behaviour.Controller.General;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using Lib.State.Scene;
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
            
            // シーンコントローラー取得
            _scenecontroller = SceneStateController.Instance;
            if (_scenecontroller == null)
                Debug.LogError("SceneStateController not found in GravAffected.");
        }
        
        private readonly Vector3 _gravity;
        private readonly Rigidbody _affectedBody;
        private readonly Transform _focusCameraTransform;
        private readonly GravType _gravType;

        private readonly SceneStateController _scenecontroller;

        // 抗力倍率
        private const float ExitDampingMultiplier = 3.0f;
        
        // 停止判定用の速度閾値
        private const float ExitStopThresholdSqr = 5;

        // 脱出処理の最大時間
        private const float ExitMaxDuration = 1.0f;
        
        private float _accelerationMultiplier;
        private const float AccelerationDuration = 1f;

        #region IGravAffectionState
        public GravAffectionState GetCurrentState => GravAffectionState.Affected;
        public GravType GravType => _gravType;

        public bool Adapting { get; private set; }
        
        private readonly bool _hasCamera;

        private float _curAngle;

        [Obsolete("Obsolete")]
        public bool Change(IGravAffectionState next, bool forceChange = false) 
        {
            // 適応中は変更不可
            if (Adapting && !forceChange)
                return false;
            
            if (next == null)
                return false;
            
            // 同じ重力方向なら変更不可
            if (next.GravType == _gravType) 
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
                    axis = GravUtils.GetGravPerpendicularUnit(prev.GravType); // 適当な軸
                    angle = 180f;
                }

                var duration = 0.25f;

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

            // 速度係数初期化
            _accelerationMultiplier = 0f;
        }

        public async Task OnExit()
        {
            var exitStartTime = Time.time;

            // 徐々に速度をゼロにする
            while (_affectedBody.linearVelocity.sqrMagnitude > ExitStopThresholdSqr)
            {
                // 抗力を賭ける
                var damping = -_affectedBody.linearVelocity * (_affectedBody.mass * ExitDampingMultiplier) - _gravity;
                _affectedBody.AddForce(damping, ForceMode.Acceleration);

                await Task.Delay(10);

                // 最大時間を超えたら強制終了
                if (Time.time - exitStartTime > ExitMaxDuration)
                    break;
            }
        }

        public void OnFixedUpdate()
        {
            if (_affectedBody == null)
                return;
            
            // ステートがInGameでない場合は重力の影響を受けない
            var curState = _scenecontroller.Context.CurrentState;
            if (curState is not { StateName: SceneState.InGame })
                return;

            // 徐々に加速度を増加させる
            _accelerationMultiplier =
                Mathf.Min(_accelerationMultiplier + Time.fixedDeltaTime / AccelerationDuration, 1f);
            
            // 重力の影響を受ける
            _affectedBody.AddForce(_gravity * _accelerationMultiplier, ForceMode.Acceleration);
        }
        #endregion
    }
}