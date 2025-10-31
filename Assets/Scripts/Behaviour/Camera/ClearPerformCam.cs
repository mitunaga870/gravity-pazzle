#region

using System;
using Behaviour.Gravity.Abstract;
using Behaviour.Player.Abstract;
using Behaviour.Trigger;
using Lib.Logic.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Camera
{
    /// <summary>
    ///     クリア演出時のカメラ用クラス
    /// </summary>
    public class ClearPerformCam : MonoBehaviour
    {
        private bool _isPerforming;

        private Vector3 _targetPosition;
        private Vector3 _axi;

        [SerializeField]
        private float height = 40f;

        [SerializeField]
        private float radius = 50f;

        [SerializeField]
        private float speed = 1f;

        #region Unity Methods

        private void Start()
        {
            // ゴールTriggerを探してコールバックを登録
            var goalTrigger = FindObjectsByType<GoalTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in goalTrigger) trigger.AddOnGoal(OnGoalReached);

            // 最初は非アクティブにしておく
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!_isPerforming) return;

            // プレイヤーの周りを回転する
            transform.RotateAround(_targetPosition, _axi, speed);

            // 常にプレイヤーの方を向く
            transform.LookAt(_targetPosition);
        }

        #endregion

        #region Private Methods

        private void OnGoalReached(APlayerBehaviour playerBehaviour, AGravBehaviour gravBehaviour)
        {
            // 演出中は何もしない
            if (_isPerforming) return;

            _isPerforming = true;
            _targetPosition = playerBehaviour.transform.position;
            _axi = GravUtils.GetGravDirectionUnit(gravBehaviour.GravType);

            // カメラの位置を設定
            var right = GravUtils.GetGravPerpendicularUnit(gravBehaviour.GravType);
            transform.position =
                _targetPosition
                - _axi * height
                + right * radius;

            // メインカメラを切り替える
            var mainCam = GameObject.FindWithTag("MainCamera");
            if (mainCam == null) throw new Exception("MainCamera not found in the scene.");
            mainCam.SetActive(false);
            gameObject.SetActive(true);
        }

        #endregion
    }
}