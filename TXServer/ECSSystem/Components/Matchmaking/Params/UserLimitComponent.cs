using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(3911401339075883957)]
    public class UserLimitComponent : Component
    {
        public UserLimitComponent(int userLimit, int teamLimit)
        {
            UserLimit = userLimit;
            TeamLimit = teamLimit;
        }
        
        public int UserLimit { get; set; }
        
        public int TeamLimit { get; set; }
    }
}