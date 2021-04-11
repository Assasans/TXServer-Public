using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1498460950985L)]
    public class CustomBattleLobbyTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(ClientBattleParams battleParams, Entity map, float gravity, Player owner)
        {
            Entity entity = new Entity(new TemplateAccessor(new CustomBattleLobbyTemplate(), null),
                map.GetComponent<MapGroupComponent>(),
                new BattleModeComponent(battleParams.BattleMode),
                new UserLimitComponent(userLimit:battleParams.MaxPlayers, teamLimit:battleParams.MaxPlayers / 2),
                new GravityComponent(gravity, battleParams.Gravity)
            );
            entity.Components.Add(new UserGroupComponent(owner.User));
            entity.Components.Add(new ClientBattleParamsComponent(battleParams));
            entity.Components.Add(new BattleLobbyGroupComponent(entity));
            
            int price = 1000;  // 1000 Blue-Crystals standard price for opening custom battles
            if (owner.IsPremium)
                price = 0;  // official premium pass feature: open custom battles for free
            entity.Components.Add(new OpenCustomLobbyPriceComponent(price));

            return entity;
        }
    }
}