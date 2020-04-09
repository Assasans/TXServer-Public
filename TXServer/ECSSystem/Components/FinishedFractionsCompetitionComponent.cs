using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1545394828752)]
    public class FinishedFractionsCompetitionComponent : Component
    {
        public Entity Winner { get; } = Fractions.Antaeus;
    }
}
