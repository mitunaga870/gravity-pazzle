#region

using ScriptableObj.Upgrade;

#endregion

namespace Lib.DataClass.PlayData
{
    public class OperationDuration : IUpgradeData
    {
        public UpgradeCategory UpgradeCategory => UpgradeCategory.Action;
        public UpgradeType UpgradeType => UpgradeType.OperationDuration;

        public readonly int Level;
        public readonly float Param;
    }
}