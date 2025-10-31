#if UNITY_EDITOR

#region

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#endregion

namespace EditorScript.ForModel
{
    /// <summary>
    /// PathAttributeが付加されたフィールドの描画
    /// 参考：https://kan-kikuchi.hatenablog.com/entry/PathAttribute_1
    /// </summary>
    [CustomPropertyDrawer(typeof(PathAttribute))]
    public class PathAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //string以外に設定されている場合はスルー
            if (property.propertyType != SerializedPropertyType.String)
            {
                return;
            }

            //D&D出来るGUIを作成、 ドロップされたオブジェクトのリストを取得
            var dropObjects = CreateDragAndDropGUI(position);

            //オブジェクトがドロップされたらパスを設定
            if (dropObjects.Count > 0)
            {
                property.stringValue = AssetDatabase.GetAssetPath(dropObjects[0]);
            }

            //現在設定されているパスを表示
            GUI.Label(position, property.displayName + " : " + property.stringValue);
        }

        //D&DのGUIを作成
        private static List<Object> CreateDragAndDropGUI(Rect rect)
        {
            var dropObjects = new List<Object>();

            //D&D出来る場所を描画
            GUI.Box(rect, "");

            //マウスの位置がD&Dの範囲になければスルー
            if (!rect.Contains(Event.current.mousePosition))
            {
                return dropObjects;
            }

            //現在のイベントを取得
            var eventType = Event.current.type;

            //ドラッグ＆ドロップで操作が 更新されたとき or 実行したとき
            if (eventType is EventType.DragUpdated or EventType.DragPerform)
            {
                //カーソルに+のアイコンを表示
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                //ドロップされたオブジェクトをリストに登録
                if (eventType == EventType.DragPerform)
                {
                    dropObjects = new List<Object>(DragAndDrop.objectReferences);

                    //ドラッグを受け付ける
                    DragAndDrop.AcceptDrag();
                }

                //イベントを使用済みにする
                Event.current.Use();
            }

            return dropObjects;
        }
    }
}
#endif
