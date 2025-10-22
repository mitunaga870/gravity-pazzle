#region

using Behaviour.Gravity.Abstract;
using Lib.State.GravAffection;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Gravity
{
    /// <summary>
    /// 可変重力の挙動を持つオブジェクトのクラス
    /// </summary>
    public class VGravBehaviour: AGravBehaviour
    {

        private bool _isFocusCameraNotNull;
        
        public IGravAffectionState CurrentGravState => GravAffectionContext.CurrentState;

        // チュートリアル用の状態コード
        // 重力を変えられたか
        public bool IsGravChanged { get; private set; }
        
        # region Unity Methods

        protected override void Start()
        {
            _isFocusCameraNotNull = focusCamera != null;
            
            base.Start();
        }

        protected override void FixedUpdate()
        {   
            GravAffectionContext.OnFixedUpdate();
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        ///     無重力状態にする
        /// </summary>
        public void SetGravFloating()
        {
            if (
                !GravAffectionContext.
                    SetState(
                        new GravFloating(
                            GravType, 
                            AffectedRigidBody,
                            transform,
                            _isFocusCameraNotNull ? focusCamera!.transform : null
            )))
                Debug.LogError("Failed to set GravFloating state.");
        }

        /// <summary>
        ///     重力を設定方向にする
        /// </summary>
        /// <param name="gravType">適用する重力タイプを設定します</param>
        /// <param name="forceChange">速度があるときに強制的に変更するかどうか</param>
        /// <param name="registerOperation">操作制限マネージャーに登録するかどうか（リセット・ギミック等は false を指定）</param>
        /// <param name="affectProps">他の重力影響を受けるオブジェクトにも影響を与えるかどうか</param>
        public virtual bool SetGravAffected(
            GravType gravType,
            bool forceChange = false,
            bool registerOperation = true,
            bool affectProps = true
        )
        {
            var previousType = GravType; // 変更前の重力（タイムアウト時の復帰に使用）
            var manager = GravityOperationManager.Instance; // 同時操作数／残時間を監視するマネージャー
            var
                handle = GravityOperationManager.OperationHandle.None; // 操作登録の結果トークン

            if (registerOperation && manager != null && !manager.IsReverting)
            {
                // 操作上限を越えていないか確認し、必要であればカウントを登録
                if (!manager.TryPrepareOperation(this, gravType, previousType, out handle))
                {
                    Debug.LogWarning($"[{name}] 重力操作の上限({manager.MaxConcurrentOperations})に達しているため変更できません。");
                    return false;
                }
            }

            var success = GravAffectionContext.SetState(
                new GravAffected(
                    gravType,
                    AffectedRigidBody,
                    _isFocusCameraNotNull ? focusCamera!.transform : null),
                forceChange
            );
            if (!success)
                // 実際の状態遷移に失敗した場合は操作登録をロールバック
                if (registerOperation && manager != null && !manager.IsReverting)
                {
                    manager.RollbackOperation(this, handle);
                    return false;
                }

            IsGravChanged = true;


            // 成功を通知し、必要であれば操作枠を開放
            if (registerOperation && manager != null && !manager.IsReverting)
                manager.NotifySuccessfulChange(this, handle);

            if (affectProps)
            {
                // 周辺オブジェクトを取得して影響を与える
                var aroundObjs = new Collider[50];
                var size = Physics.OverlapSphereNonAlloc(transform.position, 5f, aroundObjs);

                for (var i = 0; i < size; i++)
                {
                    var gravBehaviour = aroundObjs[i].GetComponent<GravProps>();
                    if (gravBehaviour != null && gravBehaviour != this)
                        gravBehaviour.SetGravAffected(gravType, forceChange, false);
                }
            }

            return true;
        }

        #endregion
    }
}