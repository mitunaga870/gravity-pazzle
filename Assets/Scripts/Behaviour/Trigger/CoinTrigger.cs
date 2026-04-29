#region

using System;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

namespace Behaviour.Trigger
{
    /// <summary>
    ///     コイントリガークラス
    ///     プレイヤーが触れるとコインを取得する
    /// </summary>
    public class CoinTrigger : MonoBehaviour
    {
        [SerializeField]
        private string coinId;

        private bool _collected;

        private void Awake()
        {
            // IDが設定されていない場合は、GameObjectのInstanceIDを使用
            if (string.IsNullOrEmpty(coinId))
                GenerateCoinId();
        }

        private void Start()
        {
            // StageDataControllerにコインを登録（Startで実行することでStageDataControllerの初期化を保証）
            if (StageDataController.Instance != null)
            {
                StageDataController.Instance.RegisterCoin(coinId);

                // 既に取得済みの場合はコインオブジェクトを非表示にする
                // ReSharper disable once InvertIf
                if (StageDataController.Instance.IsCoinCollected(coinId))
                {
                    _collected = true;
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError($"[CoinTrigger] コイン'{coinId}'をGameObject'{gameObject.name}'に登録しようとしましたが、StageDataController.Instanceがnullです。CoinTrigger.Awake()の前にStageDataControllerが初期化されていることを確認してください。");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 既に取得済みの場合は何もしない
            if (_collected) return;

            // プレイヤー以外のオブジェクトがトリガーに入った場合は何もしない
            if (!other.CompareTag("Player")) return;

            // コインを取得
            _collected = true;

            // データコントローラーにコイン取得を通知
            StageDataController.Instance.CollectCoin(coinId);
            PlayerDataController.Instance.CollectCoin(1);

            // SEを再生
            SoundController.Instance.PlaySe("Get");

            // コインオブジェクトを非表示にする
            gameObject.SetActive(false);
        }

        public void GenerateCoinId()
        {
            coinId = Guid.NewGuid().ToString();
        }

        public string CoinId => coinId;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CoinTrigger))]
    public class CoinTriggerGUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // IDを初期化するボタンを創
            if (GUILayout.Button("IDを生成"))
            {
                var trigger = (CoinTrigger)target;
                Undo.RecordObject(trigger, "Generate Coin ID");
                trigger.GenerateCoinId();
                EditorUtility.SetDirty(trigger);
            }
        }
    }
#endif
}
