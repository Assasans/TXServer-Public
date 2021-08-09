using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.ShopContainers
{
    public class EverythingContainer : StandardItemContainer
    {
        public EverythingContainer(Entity containerEntity, Player player) : base(containerEntity, player)
        {
        }

        public override List<Entity> GetRewards(Random random, int containerAmount)
        {
            List<Entity> notifications = base.GetRewards(random, containerAmount);
            if (new Random().Next(0, 100) <= 5 * containerAmount)
                notifications.Add(NewItemNotificationTemplate.CreateEntity(ContainerEntity,
                    ExtraItems.GlobalItems.Goldbonus, new Random().Next(1 * containerAmount, 3 * containerAmount)));

            return notifications;
        }
    }
}
