#region

using UnityEditor;
using UnityEngine;

#endregion

namespace EditorScript.CodeCheck
{
    /// <summary>
    ///     入力関係のコードチェックを行うクラス
    /// </summary>
    public static class CheckInput
    {
        /// <summary>
        ///     Inputのラッパー使用チェック
        /// </summary>
        /// <returns></returns>
        [MenuItem("Tools/コードチェック/詳細/Inputのラッパー使用チェック")]
        public static void CheckInputWrapperUsage()
        {
            // スクリプトを確認
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts/Behaviour" });

            // あったか
            var dontUseInput = true;

            foreach (var scriptGuid in scripts)
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

                // InputControllerは除外
                if (scriptPath.Contains("InputController")) continue;

                // スクリプトの内容を取得
                var scriptContent = script.text;

                // Inputを利用または、InputControllerを参照していない場合
                if (!scriptContent.Contains("Input.") || scriptContent.Contains("InputController Input")) continue;

                Debug.LogWarning($"Inputが利用されています: {scriptPath}");
                dontUseInput = false;
            }

            // InputControllerを参照していない場合
            if (dontUseInput)
                Debug.Log("InputControllerを参照しているスクリプトはありません。Inputのラッパーを使用しています。");
            else
                Debug.LogError("Inputのラッパーを使用していないスクリプトがあります。InputControllerを使用してください。");
        }
    }
}