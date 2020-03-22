namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1453796862447)]
    public sealed class ClientLocaleComponent : Component
    {
        [Protocol] public string LocaleCode { get; set; }
    }
}
