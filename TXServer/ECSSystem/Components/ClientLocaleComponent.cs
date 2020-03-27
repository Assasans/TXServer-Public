using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1453796862447)]
    public sealed class ClientLocaleComponent : Component
    {
        public string LocaleCode { get; set; }
    }
}
