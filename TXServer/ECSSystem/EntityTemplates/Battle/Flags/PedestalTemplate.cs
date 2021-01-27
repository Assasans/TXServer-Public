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
        public static Entity CreateEntity(Vector3 position, Entity team, Entity battle)
        {
            Entity entity = new Entity(new TemplateAccessor(new PedestalTemplate(), "battle/modes/ctf"),
                new FlagPedestalComponent(position),
                team.GetComponent<TeamGroupComponent>(),
                battle.GetComponent<BattleGroupComponent>()
            );
            return entity;
        }
    }
}