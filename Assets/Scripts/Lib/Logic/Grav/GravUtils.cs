#region

using System;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Lib.Logic.Gravity
{
    /// <summary>
    /// 重力関連のユーティリティクラス
    /// </summary>
    public static class GravUtils
    {
        /// <summary>
        /// 現在の重力加速度を３次元ベクトルで取得する
        /// </summary>
        public static Vector3 GetGravAcceleration(GravType gravType)
        {
            return GetGravDirectionUnit(gravType) * GravEnv.G;
        }
        
        /// <summary>
        /// 重力方向の単位ベクトルを取得する
        /// </summary>
        public static Vector3 GetGravDirectionUnit(GravType gravType)
        {
            return gravType switch
            {
                GravType.YNegative => Vector3.down,
                GravType.YPositive => Vector3.up,
                GravType.XNegative => Vector3.left,
                GravType.XPositive => Vector3.right,
                GravType.ZNegative => Vector3.back,
                GravType.ZPositive => Vector3.forward,
                _ => Vector3.zero
            };
        }
        
        /// <summary>
        /// 重力と直行するベクトルを取得する
        /// </summary>
        public static Vector3 GetGravPerpendicularUnit(GravType gravType)
        {
            return gravType switch
            {
                GravType.YNegative => Vector3.right,
                GravType.YPositive => Vector3.left,
                GravType.XNegative => Vector3.up,
                GravType.XPositive => Vector3.down,
                GravType.ZNegative => Vector3.up,
                GravType.ZPositive => Vector3.down,
                _ => Vector3.zero
            };
        }
        /// <summary>
        /// 向きの単位ベクトルを被重力平面に投影し、単位ベクトルを返す
        /// </summary>
        private static Vector3 ProjectToGravPlane(Vector3 direction, GravType gravType)
        {
            // 重力の向き
            var gravDir = GetGravDirectionUnit(gravType);
            // 重力の向きに対して平行なベクトルを取得
            var projDir = Vector3.ProjectOnPlane(direction, gravDir);
            // 投影したベクトルの大きさが0より大きいなら、単位ベクトルを返す
            return projDir.sqrMagnitude > 0 ? projDir.normalized : Vector3.zero;
        }
        
        /// <summary>
        /// カメラの向きと重力の向きを考慮し、移動方向ベクトルを取得する
        /// </summary>
        public static Vector3 AdjustDirectionToGrav(
            bool wInput,
            bool aInput,
            bool sInput,
            bool dInput,
            Vector3 camForward,
            Vector3 camRight,
            GravType gravType
        ) {
            // 重力の向きとカメラの向きから正面方向の単位ベクトルを取得
            var forward = ProjectToGravPlane(camForward, gravType);
            // 重力の向きとカメラの向きから右方向の単位ベクトルを取得
            var right = ProjectToGravPlane(camRight, gravType);
            
            // WASDキーの入力より移動ベクトルを計算
            var moveDirection = forward * (wInput ? 1 : 0) +
                                right * (dInput ? 1 : 0) -
                                forward * (sInput ? 1 : 0) -
                                right * (aInput ? 1 : 0);
            
            // 正規化
            if (moveDirection.sqrMagnitude > 0)
            {
                moveDirection.Normalize();
            }
            else
            {
                moveDirection = Vector3.zero;
            }
            
            return moveDirection;
        }

        /// <summary>
        /// ベクトルの最大方向をGravTypeに合わせて取得する
        /// </summary>
        public static GravType GetMaxDirection(Vector3 direction)
        {
            // 重力の向き
            if (direction.sqrMagnitude <= 0)
                throw new ArgumentException("Direction is zero vector");
            
            // 最大方向を取得
            var maxAxis = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));
            if (maxAxis == Mathf.Abs(direction.x))
            {
                return direction.x > 0 ? GravType.XPositive : GravType.XNegative;
            }
            else if (maxAxis == Mathf.Abs(direction.y))
            {
                return direction.y > 0 ? GravType.YPositive : GravType.YNegative;
            }
            else
            {
                return direction.z > 0 ? GravType.ZPositive : GravType.ZNegative;
            }
        }
        
        /// <summary>
        /// 現在の重力での上方向を取得する
        /// </summary>
        /// <param name="gravType">重力の向き</param>
        public static GravType GetUpperGravType(GravType gravType)
        {
            return gravType switch
            {
                GravType.YNegative => GravType.YPositive,
                GravType.YPositive => GravType.YNegative,
                GravType.XNegative => GravType.XPositive,
                GravType.XPositive => GravType.XNegative,
                GravType.ZNegative => GravType.ZPositive,
                GravType.ZPositive => GravType.ZNegative,
                _ => throw new ArgumentOutOfRangeException(nameof(gravType), gravType, null)
            };
        }
        
        /// <summary>
        /// 現在の重力での下方向を取得する
        /// </summary>
        /// <param name="gravType">重力の向き</param>
        public static GravType GetDownGravType(GravType gravType)
        {
            return gravType switch
            {
                GravType.YNegative => GravType.YNegative,
                GravType.YPositive => GravType.YPositive,
                GravType.XNegative => GravType.XNegative,
                GravType.XPositive => GravType.XPositive,
                GravType.ZNegative => GravType.ZNegative,
                GravType.ZPositive => GravType.ZPositive,
                _ => throw new ArgumentOutOfRangeException(nameof(gravType), gravType, null)
            };
        }

        public static (Vector3, float) GetGravToGravRotation(GravType prev, GravType next)
        {
            if (prev == next)
                return (Vector3.zero, 0f);

            // 回転軸を取得
            var prevDir = GetGravDirectionUnit(prev);
            var nextDir = GetGravDirectionUnit(next);
            var axis = Vector3.Cross(prevDir, nextDir);
            // 回転角度を取得
            var angle = Vector3.Angle(prevDir, nextDir);
            return (axis, angle);
        }

        /// <summary>
        ///     Euler角度の重力軸周りの角度だけ取得する
        /// </summary>
        public static float GetGravAxisEulerAngle(GravType gravType, Vector3 eulerAngles)
        {
            return gravType switch
            {
                GravType.YNegative => eulerAngles.y,
                GravType.YPositive => eulerAngles.y * -1,
                GravType.XNegative => eulerAngles.x,
                GravType.XPositive => eulerAngles.x * -1,
                GravType.ZNegative => eulerAngles.z,
                GravType.ZPositive => eulerAngles.z * -1,
                _ => 0f
            };
        }
    }
}