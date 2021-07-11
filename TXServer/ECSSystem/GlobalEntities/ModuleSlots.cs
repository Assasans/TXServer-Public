using System.Reflection;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item.Module.Slot;
using TXServer.ECSSystem.EntityTemplates.Item.Slot;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class ModuleSlots
    {
        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.Components.Add(new UserGroupComponent(user));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Slot1 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.WEAPON),
                new SlotUserItemInfoComponent(Slot.SLOT1, ModuleBehaviourType.ACTIVE));
            public Entity Slot2 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.WEAPON),
                new SlotUserItemInfoComponent(Slot.SLOT2, ModuleBehaviourType.ACTIVE));
            public Entity Slot3 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.WEAPON),
                new SlotUserItemInfoComponent(Slot.SLOT3, ModuleBehaviourType.PASSIVE));
            public Entity Slot4 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.TANK),
                new SlotUserItemInfoComponent(Slot.SLOT4, ModuleBehaviourType.ACTIVE));
            public Entity Slot5 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.TANK),
                new SlotUserItemInfoComponent(Slot.SLOT5, ModuleBehaviourType.ACTIVE));
            public Entity Slot6 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.TANK),
                new SlotUserItemInfoComponent(Slot.SLOT6, ModuleBehaviourType.PASSIVE));
            public Entity Slot7 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730),
                new SlotTankPartComponent(TankPartModuleType.COMMON),
                new SlotUserItemInfoComponent(Slot.SLOT7, ModuleBehaviourType.ACTIVE));
        }
    }
}
