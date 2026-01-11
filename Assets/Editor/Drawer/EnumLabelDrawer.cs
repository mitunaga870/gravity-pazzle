#region

using System;
using Lib.Attribute;
using UnityEditor;
using UnityEngine;

#endregion

namespace Drawer
{
    [CustomPropertyDrawer(typeof(EnumLabelAttribute))]
    public class EnumLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 属性からEnumの型を取得
            var enumLabelAttribute = (EnumLabelAttribute)attribute;
            var enumType = enumLabelAttribute.EnumType;

            // 配列の各要素のインデックスに対応するEnumの名前を取得
            // property.name は "Element 0" のような形式になっているが、
            // パスからインデックスを解析してEnum名に置き換える
            var path = property.propertyPath.Split('[');
            var index = Convert.ToInt32(path[path.Length - 1].Replace("]", ""));

            // Enumの要素数を超えていたら通常の表示
            var enumNames = Enum.GetNames(enumType);
            if (index >= enumNames.Length)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // ラベルをEnumの名前に差し替えて描画
            var enumName = enumNames[index];
            EditorGUI.PropertyField(position, property, new GUIContent(enumName));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}