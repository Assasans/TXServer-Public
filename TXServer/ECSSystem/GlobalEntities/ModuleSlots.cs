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

        public Entity Slot1 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.WEAPON, Slot.SLOT1, ModuleBehaviourType.ACTIVE);
        public Entity Slot2 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.WEAPON, Slot.SLOT2, ModuleBehaviourType.ACTIVE);
        public Entity Slot3 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.WEAPON, Slot.SLOT3, ModuleBehaviourType.PASSIVE);
        public Entity Slot4 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.TANK, Slot.SLOT4, ModuleBehaviourType.ACTIVE);
        public Entity Slot5 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.TANK, Slot.SLOT5, ModuleBehaviourType.ACTIVE);
        public Entity Slot6 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.TANK, Slot.SLOT6, ModuleBehaviourType.PASSIVE);
        public Entity Slot7 { get; } = SlotUserItemTemplate.CreateEntity(TankPartModuleType.COMMON, Slot.SLOT7, ModuleBehaviourType.ACTIVE);
    }
}
