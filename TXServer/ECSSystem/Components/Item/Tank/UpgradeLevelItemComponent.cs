using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components.Item.Tank
{
    [SerialVersionUID(1436343541876L)]
    public class UpgradeLevelItemComponent : Component
    {
        public UpgradeLevelItemComponent(long xp)
        {
            Level = ResourceManager.GetItemLevelByXp(xp);
        }

        public int Level { get; set; }
    }
}
