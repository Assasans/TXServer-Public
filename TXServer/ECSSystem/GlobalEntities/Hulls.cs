using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Hulls : ItemList
    {
        public static Hulls GlobalItems { get; } = new Hulls();

        public static Hulls GetUserItems(Entity user)
        {
            Hulls items = FormatterServices.GetUninitializedObject(typeof(Hulls)) as Hulls;

            foreach (PropertyInfo info in typeof(Hulls).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Dictator { get; private set; } = TankMarketItemTemplate.CreateEntity(655588521, "dictator");
        public Entity Hornet { get; private set; } = TankMarketItemTemplate.CreateEntity(532353871, "hornet");
        public Entity Hunter { get; private set; } = TankMarketItemTemplate.CreateEntity(537781597, "hunter");
        public Entity Mammoth { get; private set; } = TankMarketItemTemplate.CreateEntity(-939793870, "mammoth");
        public Entity Titan { get; private set; } = TankMarketItemTemplate.CreateEntity(-803206257, "titan");
        public Entity Viking { get; private set; } = TankMarketItemTemplate.CreateEntity(927407783, "viking");
        public Entity Wasp { get; private set; } = TankMarketItemTemplate.CreateEntity(1913834436, "wasp");
    }
}
