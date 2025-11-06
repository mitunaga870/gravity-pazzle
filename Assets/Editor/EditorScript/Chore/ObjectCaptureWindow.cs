#region

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#endregion

namespace EditorScript.Chore
{
    public class ObjectCaptureWindow : EditorWindow
    {
        // ウィンドウの設定項目
        private GameObject targetObject;
        private RenderTexture renderTexture;
        private string captureLayerName = "ObjectCapture";
        private string saveFileName = "CapturedIcon.png";
        private float padding = 1.1f; // オブジェクトの周りの余白

        // メニュー項目からウィンドウを開く
        [MenuItem("Tools/Object Capture Window")]
        public static void ShowWindow()
        {
            GetWindow<ObjectCaptureWindow>("Object Capture");
        }

        // ウィンドウのGUIを描画
        private void OnGUI()
        {
            GUILayout.Label("Object Capture Settings", EditorStyles.boldLabel);

            // 1. 撮影対象のオブジェクト
            // シーンで選択中のオブジェクトを自動で取得
            if (Selection.activeGameObject != null) targetObject = Selection.activeGameObject;
            EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

            // 2. RenderTextureの設定
            renderTexture =
                (RenderTexture)EditorGUILayout.ObjectField("Render Texture", renderTexture, typeof(RenderTexture),
                    false);

            // 3. レイヤー名とファイル名の設定
            captureLayerName = EditorGUILayout.TextField("Capture Layer", captureLayerName);
            saveFileName = EditorGUILayout.TextField("Save File Name", saveFileName);
            padding = EditorGUILayout.FloatField("Padding", padding);

            if (GUILayout.Button("Capture"))
            {
                if (targetObject == null || renderTexture == null)
                {
                    EditorUtility.DisplayDialog("Error", "Target Object または RenderTexture が設定されていません。", "OK");
                    return;
                }

                // レイヤーが存在するかチェック
                var layer = LayerMask.NameToLayer(captureLayerName);
                if (layer == -1)
                {
                    EditorUtility.DisplayDialog("Error",
                        $"レイヤー '{captureLayerName}' が存在しません。\nProject Settings > Tags and Layers で作成してください。", "OK");
                    return;
                }

                CaptureObject();
            }
        }

        private void CaptureObject()
        {
            var captureLayer = LayerMask.NameToLayer(captureLayerName);
            if (captureLayer == -1)
            {
                Debug.LogError($"レイヤー '{captureLayerName}' が存在しません。");
                return;
            }

            // --- 1. 一時的なカメラを作成 ---
            var tempCameraGO = new GameObject("Temp Capture Camera");
            var captureCamera = tempCameraGO.AddComponent<Camera>();
            captureCamera.allowHDR = false;

            var urpCamData = tempCameraGO.AddComponent<UniversalAdditionalCameraData>();

            // ★修正点 1: カメラのポストプロセスを有効化し、RenderTypeを設定
            urpCamData.renderPostProcessing = true;
            urpCamData.renderType = CameraRenderType.Base;

            // --- 2. 一時的なライトを作成 ---
            var tempLightGO = new GameObject("Temp Capture Light");
            tempLightGO.transform.SetParent(tempCameraGO.transform);
            var tempLight = tempLightGO.AddComponent<Light>();
            tempLight.type = LightType.Directional;
            tempLight.intensity = 1.2f;
            tempLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            // (ライトは Default レイヤーのまま)

            // ★修正点 2: 一時的なVolumeとBloomを作成 ★★★
            var tempVolumeGO = new GameObject("Temp Capture Volume");
            tempVolumeGO.layer = captureLayer; // ★カメラが検出できるようキャプチャレイヤーに設定

            var volume = tempVolumeGO.AddComponent<Volume>();
            volume.isGlobal = true; // グローバルVolumeとして機能
            volume.priority = 100f; // 他のVolumeより優先

            // Volume設定用のプロファイルも一時的に作成
            var profile = CreateInstance<VolumeProfile>();

            // Bloomを追加して設定
            var bloom = profile.Add<Bloom>(true); // true = 有効状態で追加
            bloom.threshold.Override(1.0f); // 発光強度(Intensity)が 1.0 より明るい部分が光る
            bloom.intensity.Override(2.0f); // 光の全体的な強さ (この値を調整)
            bloom.scatter.Override(0.7f); // 光の広がり具合 (0〜1)
            bloom.tint.Override(Color.white); // 光の色 (マテリアルの発光色に加算)

            volume.profile = profile;
            // ★★★ ここまで ★★★

            // --- 3. オブジェクトのバウンディングボックスを取得 ---
            var bounds = GetBounds(targetObject);

            // --- 4. カメラのセットアップ ---
            SetupCamera(captureCamera, bounds, renderTexture);

            // --- 5. レイヤー設定 & 撮影 ---
            var originalLayers = new Dictionary<GameObject, int>();
            SetLayerRecursively(targetObject, captureLayer, originalLayers);

            // ★修正点 3: カリングマスクの設定
            // オブジェクト(captureLayer) と Volume(captureLayer) だけを見る
            captureCamera.cullingMask = 1 << captureLayer;

            captureCamera.Render();

            // --- 6. Texture2Dに保存 ---
            SaveRenderTextureToFile(renderTexture, saveFileName);

            // --- 7. 後片付け ---
            foreach (var pair in originalLayers)
                if (pair.Key != null)
                    pair.Key.layer = pair.Value;

            DestroyImmediate(tempCameraGO); // ライトも子なので一緒に破棄される

            // ★修正点 4: 作成したVolumeとProfileを破棄
            DestroyImmediate(tempVolumeGO);
            DestroyImmediate(profile); // Profileの破棄も忘れずに

            Debug.Log($"キャプチャを保存しました: Assets/{saveFileName}");
            AssetDatabase.Refresh();
        }


        // ★★★↓ このメソッドを丸ごと置き換え ↓★★★
        private void SetupCamera(Camera cam, Bounds bounds, RenderTexture rt)
        {
            cam.targetTexture = rt;
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);

            // ★修正点: urpCamDataの設定を CaptureObject() に移動したため、ここのブロックは不要
            /*
            var urpCamData = cam.GetUniversalAdditionalCameraData();
            if (urpCamData != null)
            {
                // urpCamData.renderType = CameraRenderType.Base; // CaptureObjectに移動
            }
            */

            // --- カメラの角度と位置 ---
            var maxBoundsSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (maxBoundsSize < 0.01f) maxBoundsSize = 1.0f;

            var offsetDirection = new Vector3(1.0f, 0.7f, -1.0f).normalized;
            var distance = maxBoundsSize * 2.0f;

            cam.transform.position = bounds.center + offsetDirection * distance;
            cam.transform.LookAt(bounds.center);

            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = distance * 2.0f;

            // --- OrthographicSizeの計算 ---
            var aspect = (float)rt.width / rt.height;
            var objectSize = maxBoundsSize;

            var cameraSizeX = objectSize * 0.5f;
            var cameraSizeY = objectSize * 0.5f;

            float targetOrthographicSize;
            if (aspect >= 1.0f)
                targetOrthographicSize = Mathf.Max(cameraSizeY, cameraSizeX / aspect);
            else
                targetOrthographicSize = Mathf.Max(cameraSizeY, cameraSizeX / aspect);

            cam.orthographicSize = targetOrthographicSize * padding;
        }

        // ★★★↓ このメソッドを丸ごと置き換え ↓★★★
        // オブジェクト（と非アクティブな子も含む）の全体のBoundsを取得する
        private Bounds GetBounds(GameObject obj)
        {
            // ★修正点: 非アクティブなオブジェクトも検索対象に含める (true)
            var renderers = obj.GetComponentsInChildren<Renderer>(true);

            if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

            // ★修正点: 非アクティブなRendererのboundsは(0,0,0)を返すため、
            // それらを一時的に有効化して計算する
            var renderersToRevert = new List<Renderer>();
            foreach (var r in renderers)
                if (!r.enabled)
                {
                    r.enabled = true;
                    renderersToRevert.Add(r);
                }

            // Boundsを計算
            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

            // ★状態を元に戻す
            foreach (var r in renderersToRevert) r.enabled = false;

            return bounds;
        }

        // 再帰的にレイヤーを設定し、元のレイヤーを保存する
        private void SetLayerRecursively(GameObject obj, int layer, Dictionary<GameObject, int> originalLayers)
        {
            if (originalLayers.ContainsKey(obj)) return;

            originalLayers[obj] = obj.layer;
            obj.layer = layer;

            foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, layer, originalLayers);
        }

        // RenderTextureをPNGファイルとして保存
        private void SaveRenderTextureToFile(RenderTexture rt, string fileName)
        {
            var prevActiveRT = RenderTexture.active;
            RenderTexture.active = rt;

            var texture2D = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = prevActiveRT;

            var bytes = texture2D.EncodeToPNG();
            var fullPath = Path.Combine(Application.dataPath, fileName);

            File.WriteAllBytes(fullPath, bytes);

            DestroyImmediate(texture2D); // エディタスクリプトではDestroyImmediateを使用
        }
    }
}