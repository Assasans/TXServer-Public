using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.ShopContainers
{
    public class ScoutContainer : BlueprintContainer
    {
        public ScoutContainer(Entity entity, Player player, ContainerInfo.ContainerInfo containerInfo) : base(entity,
            player, containerInfo)
        {
        }

        public override List<Entity> GetRewards(Random random, int containerAmount)
        {
            Dictionary<Entity, int> blueprints = new Dictionary<Entity, int>();
            float blueprintAmount = random.Next(190, 250) * containerAmount;

            for (int i = 0; i < blueprintAmount; i++)
            {
                Entity blueprint = ItemListBlueprints[random.Next(ItemListBlueprints.Count)];
                blueprints = AddBlueprint(blueprint, blueprints);
            }

            return CreateNewCardsNotifications(blueprints);
        }
    }
}
