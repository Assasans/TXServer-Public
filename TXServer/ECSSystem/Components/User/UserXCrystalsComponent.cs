using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1473074767785)]
    public class UserXCrystalsComponent : Component
    {
        public UserXCrystalsComponent(long Money)
        {
            this.Money = Money;
        }

        public long Money { get; set; }
    }
}
