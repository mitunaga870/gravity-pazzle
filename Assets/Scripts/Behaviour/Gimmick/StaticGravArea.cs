#region

using Behaviour.Gravity;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Gimmick
{
    public class StaticGravArea : MonoBehaviour
    {
        [SerializeField]
        private Collider _collider;

        [SerializeField]
        private GravType _gravType = GravType.YNegative;

        // 触ってるオブジェクトの重力を変える
        private void OnTriggerStay(Collider other)
        {
            // オブジェクトのGravBehaviourを取得
            var gravBehaviour = other.GetComponent<VGravBehaviour>();

            // VGravBehaviourがnullでない場合のみ処理を行う
            if (gravBehaviour == null)
                return;

            // GravBehaviourの重力を設定する
            gravBehaviour.SetGravAffected(_gravType, true);
        }

        /// <summary>
        ///     静的な重力エリアの重力タイプを変更します。
        /// </summary>
        /// <param name="newGravType">次にせってする重力を設定する</param>
        /// <returns></returns>
        public bool ChangeGravType(GravType newGravType)
        {
            _gravType = newGravType;
            return true;
        }
    }
}