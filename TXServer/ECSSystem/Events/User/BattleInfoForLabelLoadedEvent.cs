using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(635890736905417870L)]
    public class BattleInfoForLabelLoadedEvent : ECSEvent
    {
        public BattleInfoForLabelLoadedEvent(Entity map, long battleId, string battleMode)
        {
            Map = map;
            BattleId = battleId;
            BattleMode = battleMode;
        }

        public Entity Map { get; set; }
        public long BattleId { get; set; }
        public string BattleMode { get; set; }
    }
}
