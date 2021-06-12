using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item.News
{
    [SerialVersionUID(1481290407948L)]
    public class NewsItemSaleLabelComponent : Component
    {
        public NewsItemSaleLabelComponent(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
