using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Team
{
    [SerialVersionUID(-2440064891528955383)]
    public class TeamScoreComponent : Component
    {
        public TeamScoreComponent() => Score = 0;

        public int Score { get; set; }
    }
}
