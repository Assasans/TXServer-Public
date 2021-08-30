using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(635902862624765629L)]
    public class UnconfirmedUserEmailComponent : Component
    {
        public UnconfirmedUserEmailComponent(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}
