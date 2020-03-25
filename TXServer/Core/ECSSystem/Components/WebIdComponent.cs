namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1479820450460)]
    public sealed class WebIdComponent : Component
    {
        public string WebId { get; set; } = "";
        public string Utm { get; set; } = "";
        public string GoogleAnalyticsId { get; set; } = "";
        public string WebIdUid { get; set; } = "";
    }
}