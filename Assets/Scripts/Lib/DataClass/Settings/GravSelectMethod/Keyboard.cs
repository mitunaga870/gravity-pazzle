#region

using System;

#endregion

namespace Lib.DataClass.Settings.GravSelectMethod
{
    [Serializable]
    public class Keyboard : IGravSelectMethod
    {
        public string DisplayName => "キーボードによる指定";
    }
}