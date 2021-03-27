using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636147227222284613L)]
    public class UserRankRewardNotificationInfoComponent : Component
    {
        public UserRankRewardNotificationInfoComponent(long xCrystals, long crystals, long rank)
        {
            RedCrystals = xCrystals;
            BlueCrystals = crystals;
            Rank = rank;
        }

        public long RedCrystals { get; set; }
        public long BlueCrystals { get; set; }
        public long Rank { get; set; }
    }
}
