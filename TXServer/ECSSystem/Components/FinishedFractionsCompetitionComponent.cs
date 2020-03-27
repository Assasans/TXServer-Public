using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.ECSSystem.Base.Entity;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1545394828752)]
    public class FinishedFractionsCompetitionComponent : Component
    {
        public Entity Winner { get; } = GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS;
    }
}
