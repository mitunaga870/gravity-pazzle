#region

using ScriptableObj.Upgrade;

#endregion

namespace Lib.DataClass.PlayData
{
    public interface IUpgradeData
    {
        public UpgradeCategory UpgradeCategory { get; }
        public UpgradeType UpgradeType { get; }
    }
}