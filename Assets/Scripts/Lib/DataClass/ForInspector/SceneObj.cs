#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

namespace Lib.DataClass.ForInspector
{
    [Serializable]
    public class SceneObj
    {
        [SerializeField]
        private string m_SceneName;

        public static implicit operator string(SceneObj sceneObject)
        {
            return sceneObject.m_SceneName;
        }

        public static implicit operator SceneObj(string sceneName)
        {
            return new SceneObj { m_SceneName = sceneName };
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneObj))]
    public class SceneObjEditor : PropertyDrawer
    {
        protected SceneAsset GetSceneObj(string sceneObjName)
        {
            if (string.IsNullOrEmpty(sceneObjName))
                return null;

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.path.IndexOf(sceneObjName) != -1)
                    return AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset;
            }

            Debug.Log("Scene [" + sceneObjName +
                      "] cannot be used. Add this scene to the 'Scenes in the Build' in the build settings.");
            return null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneObj = GetSceneObj(property.FindPropertyRelative("m_SceneName").stringValue);
            var newScene = EditorGUI.ObjectField(position, label, sceneObj, typeof(SceneAsset), false);
            if (newScene == null)
            {
                var prop = property.FindPropertyRelative("m_SceneName");
                prop.stringValue = "";
            }
            else
            {
                if (newScene.name != property.FindPropertyRelative("m_SceneName").stringValue)
                {
                    var scnObj = GetSceneObj(newScene.name);
                    if (scnObj == null)
                    {
                        Debug.LogWarning("The scene " + newScene.name +
                                         " cannot be used. To use this scene add it to the build settings for the project.");
                    }
                    else
                    {
                        var prop = property.FindPropertyRelative("m_SceneName");
                        prop.stringValue = newScene.name;
                    }
                }
            }
        }
    }
#endif
}