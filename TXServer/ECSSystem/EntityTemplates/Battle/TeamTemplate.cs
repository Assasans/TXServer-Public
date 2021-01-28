using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1429761302402)]
    public class TeamTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(TeamColor color, Entity battle)
        {
            Entity entity = new Entity(new TemplateAccessor(new TeamTemplate(), null),
                new TeamComponent(),
                new TeamColorComponent(color),
                battle.GetComponent<BattleGroupComponent>(),
                new TeamScoreComponent(0)
            );
            entity.Components.Add(new TeamGroupComponent(entity));

            return entity;
        }
    }
}