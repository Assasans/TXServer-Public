using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Types
{
    public class EndSeasonRewardItem
    {
        public long StartPlace { get; set; } = 0;

        public long EndPlace { get; set; } = 1000;

        [OptionalMapped] public object Padding { get; set; } = null;
    }
}
