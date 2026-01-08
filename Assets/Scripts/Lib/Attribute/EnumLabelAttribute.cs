#region

using System;
using UnityEngine;

#endregion

namespace Lib.Attribute
{
    public class EnumLabelAttribute : PropertyAttribute
    {
        public Type EnumType;

        public EnumLabelAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}