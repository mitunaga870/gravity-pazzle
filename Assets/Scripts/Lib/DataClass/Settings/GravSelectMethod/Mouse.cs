#region

using System;

#endregion

namespace Lib.DataClass.Settings.GravSelectMethod
{
    [Serializable]
    public class Mouse : IGravSelectMethod
    {
        public string DisplayName => "マウスによる指定";
    }
}