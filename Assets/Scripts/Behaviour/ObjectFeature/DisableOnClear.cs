#region

using Behaviour.Trigger;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature
{
    /// <summary>
    ///     クリア時にオブジェクトを無効化するコンポーネント
    /// </summary>
    public class DisableOnClear : MonoBehaviour
    {
        private void OnEnable()
        {
            var goalTriggers = FindObjectsByType<GoalTrigger>(FindObjectsSortMode.None);
            foreach (var goalTrigger in goalTriggers)
                goalTrigger.AddOnGoal(() => { gameObject.SetActive(false); });
        }
    }
}