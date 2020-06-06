using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1485846188251L)]
    public class SlotUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(TankPartModuleType part, Slot slot, ModuleBehaviourType type)
        {
            Entity marketItem = ExtraItems.GlobalItems.ModuleSlot;
            Entity userItem = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);

            userItem.Components.UnionWith(new Component[]
            {
                new SlotTankPartComponent(part),
                new SlotUserItemInfoComponent(slot, type)
            });

            return userItem;
        }
    }
}
