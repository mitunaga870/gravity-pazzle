#region

using UnityEngine;

#endregion

namespace Lib.Logic.Math
{
    public class MathUtils
    {
        /// <summary>
        ///     最大公約数を求める
        /// </summary>
        public static int GCD(int a, int b)
        {
            // 負の数に対応
            a = Mathf.Abs(a);
            b = Mathf.Abs(b);

            // ユークリッドの互除法
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }
    }
}