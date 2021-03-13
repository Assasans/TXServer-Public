using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1510029455297L)]
	public class BattleResultForClientEvent : ECSEvent
	{
        public BattleResultForClientEvent(BattleResultForClient battleResultForClient)
        {
            UserResultForClient = battleResultForClient;
        }

        public BattleResultForClient UserResultForClient { get; set; }
	}
}