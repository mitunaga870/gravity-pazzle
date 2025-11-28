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

            // StageDataControllerにコインを登録
            StageDataController.Instance.RegisterCoin(coinId);
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
