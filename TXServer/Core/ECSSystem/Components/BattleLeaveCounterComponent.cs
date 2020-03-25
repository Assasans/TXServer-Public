namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1502092676956)]
    public class BattleLeaveCounterComponent : Component
    {
        public long Value { get; set; }

        public int NeedGoodBattles { get; set; }
    }
}
