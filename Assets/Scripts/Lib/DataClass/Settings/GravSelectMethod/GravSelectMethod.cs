#region

using System;

#endregion

namespace Lib.DataClass.Settings.GravSelectMethod
{
    public enum GravSelectMethod
    {
        Mouse,
        Keyboard
    }

    public static class GravSelectMethodExtensions
    {
        public static IGravSelectMethod[] Methods { get; } =
        {
            new Mouse(),
            new Keyboard()
        };
        
        
        public static IGravSelectMethod ToGravSelectMethod(this GravSelectMethod method)
        {
            return method switch
            {
                GravSelectMethod.Mouse => new Mouse(),
                GravSelectMethod.Keyboard => new Keyboard(),
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };
        }
    }
}