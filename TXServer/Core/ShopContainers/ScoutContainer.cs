using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.ShopContainers
{
    public class ScoutContainer : ShopContainer
    {
        public ScoutContainer(Entity containerEntity, Player player) : base(containerEntity, player)
        {
        }

        public override List<Entity> GetRewards(Random random, int containerAmount)
        {
            Dictionary<Entity, int> blueprints = new Dictionary<Entity, int>();
            float blueprintAmount = random.Next(190, 250) * containerAmount;

            for (int i = 0; i < blueprintAmount; i++)
            {
                Entity blueprint = _possibleBlueprints[random.Next(_possibleBlueprints.Count)];
                blueprints = AddBlueprint(blueprint, blueprints);
            }

            return CreateNewCardsNotifications(blueprints);
        }

        private readonly List<Entity> _possibleBlueprints = new()
        {
            ModuleCards.GlobalItems.Explosivemass,
            ModuleCards.GlobalItems.Externalimpact,
            ModuleCards.GlobalItems.Firering,
            ModuleCards.GlobalItems.Jumpimpact,
            ModuleCards.GlobalItems.Kamikadze,
            ModuleCards.GlobalItems.Sapper
        };
    }
}
