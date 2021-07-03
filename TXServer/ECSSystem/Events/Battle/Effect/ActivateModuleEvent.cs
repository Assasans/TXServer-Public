using TXServer.Core;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Effect
{
	[SerialVersionUID(1486015564167L)]
	public class ActivateModuleEvent : ECSEvent
    {
		public void Execute(Player player, Entity entity)
        {
            BattleModule module = player.BattlePlayer.MatchPlayer.Modules.Find(m => m.ModuleEntity == entity);

            if (module is null || !module.IsEnabled || module.IsOnCooldown) return;

            module.IsSupply = module.IsCheat = false;

            module.Activate();
            module.CurrentAmmunition--;
        }

		public int ClientTime { get; set; }
	}
}
