using System;
using TXServer.Core.ECSSystem.Entities;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1545394828752)]
    public class FinishedFractionsCompetitionComponent : Component
    {
        [Protocol] public Int64 Winner { get; } = GlobalEntityIds.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS;
    }
}
