#region

using Behaviour.Player.Abstract;
using Lib.Logic.Gravity;
using UnityChan;
using UnityEngine;

#endregion

namespace Behaviour.Player
{
    /// <summary>
    ///     壁に立つプレイヤーの挙動クラス
    /// </summary>
    public class StandingWallPlayerBehaviour : APlayerBehaviour
    {
        #region Unity Methods

        private new void Start()
        {
            // 基底クラスのStartを呼び出す
            base.Start();

            // 子オブジェクトにつく、全てのSpringBoneを取得
            var children = GetComponentsInChildren<Transform>();
            if (children.Length == 0)
                Debug.LogError("No child objects found.");
            else
                foreach (var child in children)
                {
                    var springBone = child.GetComponent<SpringBone>();
                    // 重力方向を上向きに設定
                    if (springBone != null)
                    {
                        // 詳細は不明だが、UnityChanのSpringBoneは0.001f程で奇麗な重力になる
                        var gravFactor = 0.001f;
                        springBone.springForce = GravUtils.GetGravDirectionUnit(gravBehaviour.GravType) * gravFactor;
                    }
                }
        }

        private void Update()
        {
            // 基底クラスのUpdateを呼び出す
            base.Update();
        }

        #endregion


        #region APlayerBehaviour Implementation

        protected override Vector3 GetMoveDirection(float deltaTime)
        {
            // 壁に立つプレイヤーは移動しない
            return Vector3.zero;
        }

        #endregion
    }
}