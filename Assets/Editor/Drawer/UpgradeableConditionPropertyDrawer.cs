#region

using System;
using ScriptableObj.Upgrade;
using UnityEditor;
using UnityEngine;

#endregion

namespace Drawer
{
    [CustomPropertyDrawer(typeof(UpgradeableConditionValue))]
    public class UpgradeableConditionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Enumの型情報を取得（今回は ElementType 固定ですが、汎用化も可能です）
            var enumType = typeof(UpgradeType);
            var enumNames = Enum.GetNames(enumType);

            // クラスの中にある "values" 配列を探す
            var arrayProp = property.FindPropertyRelative("values");

            // ■ サイズ強制ロジック
            // 配列サイズがEnumと合わない場合、ここで強制的にリサイズします
            if (arrayProp.arraySize != enumNames.Length) arrayProp.arraySize = enumNames.Length;

            // ■ 描画ロジック
            // まずプロパティ全体のラベル（変数名）を表示
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                // インデントを下げる
                EditorGUI.indentLevel++;

                // 配列の中身を1つずつ手動で描画（Sizeフィールドは描画しない！）
                for (var i = 0; i < arrayProp.arraySize; i++)
                {
                    // 次の行へ座標をずらす
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    var elementProp = arrayProp.GetArrayElementAtIndex(i);
                    var enumName = enumNames[i];

                    // ラベルをEnum名にして表示
                    EditorGUI.PropertyField(position, elementProp, new GUIContent(enumName));
                }

                EditorGUI.indentLevel--;
            }
        }

        // プロパティの高さを計算（開いているときは要素数分だけ高くする）
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            var arrayProp = property.FindPropertyRelative("values");
            // ヘッダー1行 + (要素の高さ + 隙間) * 要素数
            return EditorGUIUtility.singleLineHeight +
                   (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * arrayProp.arraySize;
        }
    }
}