using TXServer.Core;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Effect.EMP
{
    [SerialVersionUID(636250863918020313L)]
    public class ApplyTargetsForEMPEffectEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (Targets.Length <= 0) return;
            if (player.BattlePlayer.MatchPlayer.TryGetModule(out EmpModule empModule))
                empModule.ApplyEmpOnTargets(Targets);
        }

        public Entity[] Targets { get; set; }
    }
}
