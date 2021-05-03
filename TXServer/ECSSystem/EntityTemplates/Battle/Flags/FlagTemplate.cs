using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1431941266589L)]
    public class FlagTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Vector3 position, Entity team, Entity battle)
        {
            return new(new TemplateAccessor(new FlagTemplate(), "battle/modes/ctf"),
                new FlagComponent(),
                team.GetComponent<TeamGroupComponent>(),
                battle.GetComponent<BattleGroupComponent>(),
                new FlagHomeStateComponent(),
                new FlagPositionComponent(position));
        }
    }
}