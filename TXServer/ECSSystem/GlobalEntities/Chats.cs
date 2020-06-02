using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Chats : ItemList
    {
        public static Chats GlobalItems { get; } = new Chats();

        public Entity Ru { get; } = new Entity(-968268831, new TemplateAccessor(new GeneralChatTemplate(), "/chat/general/ru"),
            new GeneralChatComponent(),
            new ChatComponent());
    }
}
