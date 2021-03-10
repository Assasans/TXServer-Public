using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Entrance
{
    [SerialVersionUID(1479198715562L)]
    public sealed class RestorePasswordCodeSentComponent : Component
    {
        public RestorePasswordCodeSentComponent(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}
