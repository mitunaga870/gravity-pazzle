#region

using Behaviour.Controller.Stage;
using Behaviour.Trigger;
using Lib.Logic;
using TMPro;
using UnityEngine;

#endregion

namespace Behaviour.UI.InGame
{
    public class ClearUIWrapper : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text clearTimeTextMain;

        [SerializeField]
        private TMP_Text clearTimeTextSub;

        private StageDataController StageDataController => StageDataController.Instance;

        private void Start()
        {
            var goalTriggers = FindObjectsByType<GoalTrigger>(FindObjectsSortMode.None);
            foreach (var goalTrigger in goalTriggers) goalTrigger.AddOnGoal(OnGoal);

            gameObject.SetActive(false);
        }

        private void OnGoal()
        {
            var playTime = StageDataController.PlayTime;
            clearTimeTextMain.text = GeneralUtils.TimeSpanToMinuteSecondString(playTime);
            clearTimeTextSub.text = ":" + GeneralUtils.TimeSpanToMilliSec(playTime).ToString("D2");

            gameObject.SetActive(true);
        }
    }
}