using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1431942342249L)]
    public class PedestalTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(TeamColor color, Vector3 position, Entity team)
        {
            Entity entity = new Entity(new TemplateAccessor(new PedestalTemplate(), "battle/modes/ctf"),
                new FlagPedestalComponent(position)
            );
            entity.Components.Add(new TeamGroupComponent(team));
            entity.Components.Add(new BattleGroupComponent(entity));
            return entity;
        }
    }
}