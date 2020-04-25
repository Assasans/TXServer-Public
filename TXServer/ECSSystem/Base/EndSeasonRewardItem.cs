using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Base
{
    public class EndSeasonRewardItem
    {
        public long StartPlace { get; set; } = 0;

        public long EndPlace { get; set; } = 1000;

        [OptionalMapped] object padding { get; set; } = null;
    }
}
