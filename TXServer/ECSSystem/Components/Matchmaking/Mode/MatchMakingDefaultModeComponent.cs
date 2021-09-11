using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1499242904641L)]
    public class MatchMakingDefaultModeComponent : Component
    {
        public int MinimalBattles { get; set; } = 0;
    }
}
