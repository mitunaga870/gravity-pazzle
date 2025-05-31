#region

using Behaviour.Gravity;                   // GravType 定義の名前空間
using Lib.State.Interface.Gravity;         // GravType enum の名前空間
using UnityEngine;

#endregion
namespace Behaviour.Gimmick
{
    /// <summary>
    /// スイッチ用オブジェクトにアタッチ。プレイヤーが近づいてEキーを押すと
    /// 指定した StaticGravArea の重力タイプを切り替えます。
    /// </summary>
    public class StaticGravAreaSwitch : MonoBehaviour
    {
        [SerializeField]
        private StaticGravArea _targetGravArea; // 切り替え対象のStaticGravAreaオブジェクトのStaticGravArea.cs

        [SerializeField]
        private GravType _gravType = GravType.YPositive; // スイッチを押したときにセットする重力方向

        private bool _playerInRange; // プレイヤーがトリガー内にいるかどうか


        /// <summary>
        /// プレイヤーがトリガー領域に侵入したときに呼ばれる
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }
        }

        /// <summary>
        /// プレイヤーがトリガー領域から退出したときに呼ばれる
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
            }
        }

        /// <summary>
        /// 範囲内かつEキー押下で重力タイプを切り替え
        /// </summary>
        private void Update()
        {
            // 範囲内かつEを押した瞬間
            if (_playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                if (_targetGravArea != null)
                {
                    // StaticGravAreaの重力タイプを変更
                    _targetGravArea.ChangeGravType(_gravType);
                    Debug.Log($"[Switch] {_targetGravArea.name} の重力を {_gravType} に切り替え");
                }
                else
                {
                    Debug.LogWarning("[Switch] 切り替え対象の StaticGravArea が設定されていません");
                }
            }
        }
    }
}