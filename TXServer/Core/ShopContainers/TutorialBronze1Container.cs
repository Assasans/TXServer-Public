using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.ShopContainers
{
    public class TutorialBronze1Container : BlueprintContainer
    {
        public TutorialBronze1Container(Entity entity, Player player, ContainerInfo.ContainerInfo containerInfo) : base(
            entity, player, containerInfo)
        {
        }

        public override List<Entity> GetRewards(Random random, int containerAmount) =>
            CreateNewCardsNotifications(ContainerContent);

        private static readonly Dictionary<Entity, int> ContainerContent = new()
        {
            { ModuleCards.GlobalItems.Mine, 3 },
            { ExtraItems.GlobalItems.Crystal, 50 }
        };
    }
}
