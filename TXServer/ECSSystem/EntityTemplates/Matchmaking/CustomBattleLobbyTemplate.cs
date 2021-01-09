using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1498460950985L)]
    public class CustomBattleLobbyTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity map, long mapId, int maxPlayers, BattleMode battleMode, int timeLimit, int scoreLimit, bool friendlyFire, 
            float gravity, GravityType gravityType, bool killZoneEnabled,bool disabledModules)
        {
            Entity entity = new Entity(new TemplateAccessor(new CustomBattleLobbyTemplate(), null),
                map.GetComponent<MapGroupComponent>(),
                new BattleModeComponent(battleMode),
                new UserLimitComponent(userLimit:maxPlayers, teamLimit:maxPlayers / 2),
                new GravityComponent(gravity, gravityType)
            );
            entity.Components.Add(new UserGroupComponent(entity));
            entity.Components.Add(new BattleLobbyGroupComponent(entity));
            ClientBattleParams customParams = new TXServer.ECSSystem.Types.ClientBattleParams(BattleMode:battleMode, MapId:mapId, MaxPlayers:maxPlayers, TimeLimit:timeLimit, 
                ScoreLimit:scoreLimit, FriendlyFire:friendlyFire, Gravity:gravityType, KillZoneEnabled:killZoneEnabled, DisabledModules:disabledModules);
            entity.Components.Add(new ClientBattleParamsComponent(customParams));
            return entity;
        }
    }
}