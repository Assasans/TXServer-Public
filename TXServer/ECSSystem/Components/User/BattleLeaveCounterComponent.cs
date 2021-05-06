using System.ComponentModel.DataAnnotations.Schema;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.User
{
    [SerialVersionUID(1502092676956)]
    public class BattleLeaveCounterComponent : Component
    {
        public BattleLeaveCounterComponent(long value, int needGoodBattles)
        {
            Value = value;
            NeedGoodBattles = needGoodBattles;
        }

        public long Value { get; set; }
        public int NeedGoodBattles { get; set; }

        [ProtocolIgnore] public int GoodBattlesInRow { get; set; }
    }
}
