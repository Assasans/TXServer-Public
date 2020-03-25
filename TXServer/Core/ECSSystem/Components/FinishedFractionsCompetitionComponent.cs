using static TXServer.Core.ECSSystem.Entity;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1545394828752)]
    public class FinishedFractionsCompetitionComponent : Component
    {
        public Entity Winner { get; } = GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS;
    }
}
