using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Bonus
{
	[SerialVersionUID(-4179984519411113540L)]
	public class BonusTakingRequestEvent : ECSEvent
	{
		public void Execute(Player player, Entity bonus, Entity tank)
        {
            Core.Battles.Battle battle = player.BattlePlayer.Battle;

            BattleBonus battleBonus = battle.BattleBonuses.Single(b => b.BonusEntity == bonus);

            battleBonus.Take(player);

            if (battleBonus.BonusType == BonusType.GOLD && battle.WaitingGoldBoxSenders.Any())
            {
                battle.DropSpecificBonusType(BonusType.GOLD, battle.WaitingGoldBoxSenders[0]);
                battle.WaitingGoldBoxSenders.Remove(battle.WaitingGoldBoxSenders[0]);
            }
        }
	}
}
