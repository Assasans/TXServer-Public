namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1453796862447)]
    public sealed class ClientLocaleComponent : Component
    {
        public string LocaleCode { get; set; }
    }
}
