using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1508823738925L)]
    public class CurrentSeasonNumberComponent : Component
    {
        public int SeasonNumber { get; set; } = 0;
    }
}
