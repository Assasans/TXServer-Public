using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(9171752353079252620L)]
    public class UserMoneyComponent : Component
    {
        public UserMoneyComponent(long Money)
        {
            this.Money = Money;
        }

        public long Money { get; set; }
    }
}
