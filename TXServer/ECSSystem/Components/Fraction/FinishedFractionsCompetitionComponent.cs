using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Fraction
{
    [SerialVersionUID(1545394828752)]
    public class FinishedFractionsCompetitionComponent : Component
    {
        public FinishedFractionsCompetitionComponent(long winner)
        {
            Winner = winner;
        }

        public long Winner { get; set; }
    }
}
