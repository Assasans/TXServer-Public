using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1495541167479)]
    public class MatchMakingLobbyTemplate : BattleLobbyTemplate
    {
        public static Entity CreateEntity(Entity map, BattleMode mode, int userLimit, float gravity, GravityType gravityType)
        {
            Entity entity = new Entity(new TemplateAccessor(new MatchMakingLobbyTemplate(), null),
                map.GetComponent<MapGroupComponent>(),
                new BattleModeComponent(mode),
                new UserLimitComponent(userLimit, userLimit / 2),
                new GravityComponent(gravity, gravityType),
                new MatchMakingLobbyStartTimeComponent(new TimeSpan(0, 0, 5))
                // new MatchMakingLobbyStartingComponent()
            );
            entity.Components.Add(new BattleLobbyGroupComponent(entity));
            return entity;
        }
    }
}