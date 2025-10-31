#if UNITY_EDITOR

#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace EditorScript.ForModel
{
    public class MaterialExtractor : EditorWindow
    {
        [Path] public string TargetPath = "Assets/Models/";
        public string Suffix = "materials";
        public bool IncludeChildDirectories = true;

        [MenuItem("Window/MaterialExtractor")]
        public static MaterialExtractor ShowWindow()
        {
            return GetWindow<MaterialExtractor>(nameof(MaterialExtractor));
        }

        private void OnGUI()
        {
            var so = new SerializedObject(this);
            so.Update();
            EditorGUILayout.PropertyField(so.FindProperty(nameof(TargetPath)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(Suffix)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(IncludeChildDirectories)), true);
            so.ApplyModifiedProperties();

            if (GUILayout.Button("マテリアルを抽出"))
            {
                var models = GetModels(TargetPath);
                models.ForEach(ExtractMaterials);
            }

            if (GUILayout.Button("抽出されたマテリアルをモデルに復元"))
            {
                var models = GetModels(TargetPath);
                models.ForEach(RestoreMaterials);
            }
        }

        private void ExtractMaterials(string modelPath)
        {
            var materials = AssetDatabase
                .LoadAllAssetsAtPath(modelPath)
                .Where(x => x is Material)
                .ToArray();

            Debug.Log($"{modelPath} has {materials.Length} materials.");
            if (materials.Length == 0) return;

            var assetsToReload = new HashSet<string> { modelPath };

            // ==============================================================
            // ★変更点：出力先を固定フォルダにまとめる
            // ==============================================================
            string destinationPath = "Assets/Material/";

            // フォルダが存在しない場合は作成
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
                AssetDatabase.Refresh();
            }

            foreach (var material in materials)
            {
                // 元モデル名_マテリアル名.mat にすることで衝突を防ぐ
                string fileName = $"{Path.GetFileNameWithoutExtension(modelPath)}_{material.name}.mat";
                string newAssetPath = Path.Combine(destinationPath, fileName);

                newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
                assetsToReload.Add(newAssetPath);

                var error = AssetDatabase.ExtractAsset(material, newAssetPath);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"[MaterialExtractor] error: {error}");
                }
            }

            foreach (var path in assetsToReload)
            {
                AssetDatabase.WriteImportSettingsIfDirty(path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        private void RestoreMaterials(string modelPath)
        {
            // 復元は特に触らない（元の構造を保つ）
            var materialPath = $"{Path.GetDirectoryName(modelPath)}/{Path.GetFileNameWithoutExtension(modelPath)}_{Suffix}";
            if (!Directory.Exists(materialPath)) return;

            File.Delete(materialPath + ".meta");
            Directory.Delete(materialPath, true);

            var externalObjectKeys = AssetImporter.GetAtPath(modelPath).GetExternalObjectMap().Keys;
            foreach (var key in externalObjectKeys)
            {
                AssetImporter.GetAtPath(modelPath).RemoveRemap(key);
            }

            AssetDatabase.WriteImportSettingsIfDirty(modelPath);
            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        private List<string> GetModels(string path)
        {
            var models = new List<string>();

            foreach (var fileInfo in new DirectoryInfo(path).GetFiles())
            {
                var importedObject = AssetDatabase.LoadAssetAtPath($"{path}/{fileInfo.Name}", typeof(Object));
                if (importedObject == null) continue;
                var prefabType = PrefabUtility.GetPrefabAssetType(importedObject);

                if (prefabType == PrefabAssetType.Model)
                {
                    models.Add($"{path}/{fileInfo.Name}");
                }
            }

            if (!IncludeChildDirectories) return models;

            foreach (var directoryInfo in new DirectoryInfo(path).GetDirectories())
            {
                models.AddRange(GetModels($"{path}/{directoryInfo.Name}"));
            }

            return models;
        }
    }
}
#endif
