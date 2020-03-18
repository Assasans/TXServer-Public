namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1479820450460)]
    public sealed class WebIdComponent : Component
    {
        [Protocol] public string WebId { get; set; } = "";
        [Protocol] public string Utm { get; set; } = "";
        [Protocol] public string GoogleAnalyticsId { get; set; } = "";
        [Protocol] public string WebIdUid { get; set; } = "";
    }
}