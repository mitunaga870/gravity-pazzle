using Behaviour.Trigger;
using Lib.Logic;
using UnityEngine;

namespace Behaviour.ObjectFeature.RideableObjectBehaviours
{
    public class RideOnce : MonoBehaviour
    {
        [SerializeField]
        private RideTrigger[] rideTriggers;
        [SerializeField]
        private float rideTime = 5f;
        
        private void Start()
        {
            foreach (var trigger in rideTriggers)
            {
                trigger.OnRiderEnter += _ =>
                {
                    var disableCoroutine = GeneralUtils.DelayCoroutine(
                        rideTime,
                        () =>
                        {
                            // 乗ったらコライダーを無効化
                            gameObject.SetActive(false);
                        });
                    
                    StartCoroutine(disableCoroutine);
                };
            }
        }
    }
}