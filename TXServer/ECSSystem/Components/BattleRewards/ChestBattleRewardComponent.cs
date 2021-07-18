using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.BattleRewards
{
    [SerialVersionUID(636390744977660302L)]
    public class ChestBattleRewardComponent : Component
    {
        public ChestBattleRewardComponent(Entity chest)
        {
            this.Chest = chest;
        }

        public Entity Chest { get; set; }
    }
}
