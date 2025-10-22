#region

using System.Collections;
using Lib.Logic;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Gravity.Abstract
{
    /// <summary>
    ///     周辺オブジェクトから重力影響を受けるオブジェクト
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GravProps : VGravBehaviour
    {
        [SerializeField]
        private float returnToInitialGravDelayMs = 500f;

        private IEnumerator _returnToInitialGravCoroutine;

        private GravType _initialGravType;

        protected override void Start()
        {
            _initialGravType = InitialGravType;
            base.Start();
        }

        /// <summary>
        ///     重力を設定方向にする
        ///     第一引数のみ利用されます
        /// </summary>
        /// <param name="gravType">適用する重力タイプを設定します</param>
        /// <param name="forceChange">速度があるときに強制的に変更するかどうか</param>
        /// <param name="registerOperation">操作制限マネージャーに登録するかどうか</param>
        /// <param name="affectProps">他の重力影響を受けるオブジェクトにも影響を与えるかどうか</param>
        // ReSharper disable OptionalParameterHierarchyMismatch
        public override bool SetGravAffected(
            GravType gravType,
            bool forceChange = false,
            bool registerOperation = false,
            bool affectProps = false
        )
        {
            var success = base.SetGravAffected(gravType, true, false, false);

            if (!success) return false;

            if (gravType != _initialGravType)
            {
                // 重力が初期状態と異なる場合、一定時間後に初期重力に戻すコルーチンを開始する
                // 既に戻すコルーチンが動いている場合は停止する
                if (_returnToInitialGravCoroutine != null)
                    StopCoroutine(_returnToInitialGravCoroutine);

                // 一定時間経過後に初期重力に戻す
                var durationSec = returnToInitialGravDelayMs / 1000f;
                _returnToInitialGravCoroutine = GeneralUtils.DelayCoroutine(
                    durationSec,
                    () => base.SetGravAffected(_initialGravType, true, false));

                StartCoroutine(_returnToInitialGravCoroutine);
            }
            else if (_returnToInitialGravCoroutine != null)
            {
                // 重力が初期状態と同じ場合、戻すコルーチンが動いていれば停止する
                StopCoroutine(_returnToInitialGravCoroutine);
                _returnToInitialGravCoroutine = null;
            }

            return true;
        }
        // ReSharper restore OptionalParameterHierarchyMismatch
    }
}