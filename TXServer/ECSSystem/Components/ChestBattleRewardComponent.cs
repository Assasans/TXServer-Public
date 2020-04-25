using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636390744977660302L)]
    public class ChestBattleRewardComponent : Component
    {
        public ChestBattleRewardComponent(Entity Chest)
        {
            this.Chest = Chest;
        }

        public Entity Chest { get; set; }
    }
}
