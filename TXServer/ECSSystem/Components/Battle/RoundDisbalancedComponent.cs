using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(3051892485776042754L)]
    public class RoundDisbalancedComponent : Component
    {
        public RoundDisbalancedComponent(TeamColor Loser, int InitialDominationTimerSec, TXDate FinishTime)
        {
            this.Loser = Loser;
            this.InitialDominationTimerSec = InitialDominationTimerSec;
            this.FinishTime = FinishTime;
        }

        public TeamColor Loser { get; set; }
        public int InitialDominationTimerSec { get; set; }
        public TXDate FinishTime { get; set; }
    }
}