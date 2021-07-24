using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components.Notification.FractionsCompetition
{
    [SerialVersionUID(1544689451885L)]
    public class FractionsCompetitionStartNotificationComponent : Component
    {
        public long[] FractionsInCompetition { get; set; } = {
            Fractions.GlobalItems.Antaeus.EntityId, Fractions.GlobalItems.Frontier.EntityId
        };
    }
}
