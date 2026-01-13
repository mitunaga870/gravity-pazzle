#region

using UnityEngine;

#endregion

namespace ScriptableObj.Setting
{
    [CreateAssetMenu(fileName = "SettingDescriptionText", menuName = "ScriptableObj/設定/設定の概要テキスト")]
    public class SettingDescription : ScriptableObject
    {
        [SerializeField]
        private string resolutionDescription;

        public string ResolutionDescription => resolutionDescription;

        [SerializeField]
        private string masterVolumeDescription;

        public string MasterVolumeDescription => masterVolumeDescription;

        [SerializeField]
        private string bgmVolumeDescription;

        public string BgmVolumeDescription => bgmVolumeDescription;

        [SerializeField]
        private string seVolumeDescription;

        public string SeVolumeDescription => seVolumeDescription;

        [SerializeField]
        private string tutorialToggleDescription;

        public string TutorialToggleDescription => tutorialToggleDescription;

        [SerializeField]
        private string gravSelectMethodDescription;

        public string GravSelectMethodDescription => gravSelectMethodDescription;

        [SerializeField]
        private string resetDescription;

        public string ResetDescription => resetDescription;
    }
}