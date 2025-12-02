#region

using Behaviour.Controller.Stage;
using UnityEngine;

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
                coinId = gameObject.GetInstanceID().ToString();
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
            StageDataController.Instance.CollectCoin(coinId);

            // コインオブジェクトを非表示にする
            gameObject.SetActive(false);
        }

        public string CoinId => coinId;
    }
}
