using UnityEngine;

namespace Behaviour.Trigger
{
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField]
        private GameObject goalText;
        
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