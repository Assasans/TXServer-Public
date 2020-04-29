using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Chats
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Ru { get; } = new Entity(-968268831, new TemplateAccessor(new GeneralChatTemplate(), "/chat/general/ru"),
                new GeneralChatComponent(),
                new ChatComponent());
        }
    }
}
