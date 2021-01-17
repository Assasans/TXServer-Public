using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Types;
using TXServer.Core;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1431941266589L)]
    public class FlagTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Vector3 position, Entity team, Component flagState)
        {
            Entity entity = new Entity(new TemplateAccessor(new FlagTemplate(), "battle/modes/ctf"),
                new FlagPositionComponent(position)
            );
            entity.Components.Add(new TeamGroupComponent(team));
            entity.Components.Add(flagState);

            entity.Components.Add(new BattleGroupComponent(entity));
            return entity;
        }
    }
}