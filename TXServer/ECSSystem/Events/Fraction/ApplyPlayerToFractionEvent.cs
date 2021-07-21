using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.Fraction
{
    [SerialVersionUID(1545287215097L)]
    public class ApplyPlayerToFractionEvent : ECSEvent
    {
        public void Execute(Player player, Entity fraction, Entity user)
        {
            if (player.Data.Fraction is not null) return;

            player.Data.Fraction = fraction;
            if (!user.HasComponent<FractionGroupComponent>())
                user.AddComponent(fraction.GetComponent<FractionGroupComponent>());

            if (fraction.EntityId == Fractions.GlobalItems.Antaeus.EntityId)
                Server.Instance.ServerData.AntaeusUserCount++;
            else
                Server.Instance.ServerData.FrontierUserCount++;
        }
    }
}
