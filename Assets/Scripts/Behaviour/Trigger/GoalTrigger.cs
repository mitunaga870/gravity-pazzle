#region

using UnityEngine;

#endregion

namespace Behaviour.Trigger
{
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField]
        private GameObject goalText;

        /// <summary>
        ///     ゴールに到達したときの処理
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // ゴールに到達した場合の処理
                Debug.Log("Goal Reached!");
                
                // ここでゲームクリアの処理を追加することができます
                goalText.SetActive(true);
            }
        }
    }
}