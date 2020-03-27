using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1544590059379)]
    public class FractionUserScoreComponent : Component
    {
        public FractionUserScoreComponent(long TotalEarnedPoints)
        {
            this.TotalEarnedPoints = TotalEarnedPoints;
        }

        public long TotalEarnedPoints { get; set; }
    }
}
