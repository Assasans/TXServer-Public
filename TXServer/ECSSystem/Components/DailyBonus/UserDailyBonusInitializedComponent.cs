using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636459225080719742)]
    public class UserDailyBonusInitializedComponent : Component
    {
        public UserDailyBonusInitializedComponent(Player selfOnlyPlayer) => SelfOnlyPlayer = selfOnlyPlayer;
    }
}
