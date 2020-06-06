using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Chats : ItemList
    {
        public static Chats GlobalItems { get; } = new Chats();

        public Entity Ru { get; } = GeneralChatTemplate.CreateEntity(-968268831, "ru");
    }
}
