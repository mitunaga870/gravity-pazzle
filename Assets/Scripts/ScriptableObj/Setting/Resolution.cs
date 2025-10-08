#region

using Lib.Logic.Math;
using UnityEngine;

#endregion

namespace ScriptableObj.Setting
{
    /// <summary>
    ///     ユーザー設定の解像度プリセット
    /// </summary>
    [CreateAssetMenu(fileName = "解像度プリセット", menuName = "ScriptableObj/解像度プリセット", order = 1)]
    public class Resolution : ScriptableObject
    {
        [SerializeField]
        private int width;

        [SerializeField]
        private int height;

        [SerializeField]
        private FullScreenMode fullscreenMode;

        public int Width => width;
        public int Height => height;
        public FullScreenMode FullscreenMode => fullscreenMode;

        public string AspectRatioString
        {
            get
            {
                var gcd = MathUtils.GCD(width, height);
                return $"{width / gcd}:{height / gcd}";
            }
        }

        public string DisplayString =>
            $"{fullscreenMode}：{width} x {height} ({AspectRatioString})";
    }
}