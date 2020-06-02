using System.Reflection;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class ModuleSlots : ItemList
    {
        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = new ModuleSlots();

            foreach (PropertyInfo info in typeof(ModuleSlots).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.Components.Add(new UserGroupComponent(user));
            }

            return items;
        }

        public Entity Slot1 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.WEAPON),
            new SlotUserItemInfoComponent(Slot.SLOT1, ModuleBehaviourType.ACTIVE));
        public Entity Slot2 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.WEAPON),
            new SlotUserItemInfoComponent(Slot.SLOT2, ModuleBehaviourType.ACTIVE));
        public Entity Slot3 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.WEAPON),
            new SlotUserItemInfoComponent(Slot.SLOT3, ModuleBehaviourType.PASSIVE));
        public Entity Slot4 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.TANK),
            new SlotUserItemInfoComponent(Slot.SLOT4, ModuleBehaviourType.ACTIVE));
        public Entity Slot5 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.TANK),
            new SlotUserItemInfoComponent(Slot.SLOT5, ModuleBehaviourType.ACTIVE));
        public Entity Slot6 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.TANK),
            new SlotUserItemInfoComponent(Slot.SLOT6, ModuleBehaviourType.PASSIVE));
        public Entity Slot7 { get; } = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "garage/module/slot/market"),
            new MarketItemGroupComponent(ExtraItems.GlobalItems.ModuleSlot),
            new SlotTankPartComponent(TankPartModuleType.COMMON),
            new SlotUserItemInfoComponent(Slot.SLOT7, ModuleBehaviourType.ACTIVE));
    }
}
