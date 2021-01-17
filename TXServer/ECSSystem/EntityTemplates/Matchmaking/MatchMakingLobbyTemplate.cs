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
        public static Entity CreateEntity(ClientBattleParams battleParams, Entity map, float gravity)
        {
            Entity entity = new Entity(new TemplateAccessor(new MatchMakingLobbyTemplate(), null),
                map.GetComponent<MapGroupComponent>(),
                new BattleModeComponent(battleParams.BattleMode),
                new UserLimitComponent(userLimit:battleParams.MaxPlayers, teamLimit:battleParams.MaxPlayers / 2),
                new GravityComponent(gravity, battleParams.Gravity)
                // new MatchMakingLobbyStartingComponent()
            );
            entity.Components.Add(new BattleLobbyGroupComponent(entity));
            return entity;
        }
    }
}