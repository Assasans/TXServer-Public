using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Fraction
{
    [SerialVersionUID(1545106623033L)]
    public class FractionInvolvedInCompetitionComponent : Component
    {
        public FractionInvolvedInCompetitionComponent(long userCount)
        {
            UserCount = userCount;
        }

        public long UserCount { get; set; }
    }
}
