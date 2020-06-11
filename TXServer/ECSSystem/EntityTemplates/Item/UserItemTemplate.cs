using System;
using System.Linq;
using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    public abstract class UserItemTemplate : IEntityTemplate
    {
        private static PropertyInfo GetUserItemProperty(Type type, Entity item)
        {
            ItemList list = type.GetProperty("GlobalItems").GetValue(null) as ItemList;

            PropertyInfo marketItemProperty = (from property in list.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               where (property.GetValue(list) as Entity).TemplateAccessor.ConfigPath == item.TemplateAccessor.ConfigPath
                                               select property).SingleOrDefault();

            return type.GetMethod("GetUserItems").ReturnType.GetProperty(marketItemProperty.Name);
        }

        protected static Entity GetExistingUserItem(Type itemListType, Entity marketItem)
        {
            Player.Instance.UserItems.TryGetValue(itemListType, out ItemList list);
            if (list == null) return null;

            return GetUserItemProperty(itemListType, marketItem).GetValue(Player.Instance.UserItems[itemListType]) as Entity;
        }

        protected static void AddToUserItems(Type type, Entity userItem)
        {
            Player.Instance.UserItems.TryGetValue(type, out ItemList list);
            if (list == null) return;

            GetUserItemProperty(type, userItem).SetValue(list, userItem);
        }
    }
}
