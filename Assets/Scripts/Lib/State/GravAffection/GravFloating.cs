using System;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using LitMotion;
using UnityEngine;

namespace Lib.State.GravAffection
{
    public class GravFloating : IGravAffectionState
    {
        public GravFloating(
            GravType gravType,
            Rigidbody affectedBody,
            Transform affectedTransform,
            Transform focusCameraTransform = null
        ){
            _affectedBody = affectedBody;
            _gravType = gravType;
            _affectedTransform = affectedTransform;
            _focusCameraTransform = focusCameraTransform;
        }
        
        private readonly GravType _gravType;
        private readonly Rigidbody _affectedBody;
        private readonly Transform _affectedTransform;
        private readonly Transform _focusCameraTransform;
        
        private bool _isEntered = false;
        
        #region IGravAffectionState
        public GravAffectionState GetCurrentState => GravAffectionState.Floating;
        public GravType GravType => _gravType;

        public bool Change(IGravAffectionState next)
        {
            // 停止してないと呼び出せない
            if (_affectedBody.linearVelocity.magnitude > 0.1f)
                return false;
            
            // 無重力状態以外なら実装
            return next.GetCurrentState != GravAffectionState.Unaffected;
        }
        public void OnEnter(IGravAffectionState prev)
        {
            if (_affectedBody == null)
                return;
            
            // 停止までの時間を設定
            var stopTime = 1f;
            // 速度を0にするようtweenを設定
            LMotion.Create(_affectedBody.linearVelocity, Vector3.zero, stopTime)
                .WithOnComplete(() => { _isEntered = true; })
                .Bind((velocity) =>
                {
                    // 速度を設定
                    _affectedBody.linearVelocity = velocity;
                });
        }

        public void OnExit()
        {
            
        }

        public void OnFixedUpdate()
        {
            if (_affectedBody == null)
                return;
            // 初期化済み出なけれバ
            if (!_isEntered)
                return;
            
            // ふわふわさせる（正弦波
            // ふわふわの幅
            var scale = 0.01f;
            // ふわふわの周波数
            var frequency = 1f;
            // 変異を計算
            var sin = 
                GravUtils.GetGravDirectionUnit(_gravType) 
                * (Mathf.Sin((float)(2f * Math.PI * frequency * Time.time)) * scale);
            // 変異を適用
            _affectedBody.MovePosition(_affectedTransform.position + sin);
        }
        #endregion
    }
}