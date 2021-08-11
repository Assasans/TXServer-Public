using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.ShopContainers
{
    public class BlueprintContainer : ShopContainer
    {
        public BlueprintContainer(Entity entity, Player player, ContainerInfo.ContainerInfo containerInfo) : base(
            entity, player)
        {
            ContainerInfo = containerInfo;
            TargetTierComponent = Config.GetComponent<TargetTierComponent>(MarketItemPath, false);

            if (TargetTierComponent?.ItemList is not null)
                ItemListBlueprints =
                    TargetTierComponent.ItemList.Select(number => Player.GetEntityById(number)).ToList();
        }

        public override List<Entity> GetRewards(Random random, int containerAmount)
        {
            Dictionary<Entity, int> rewardedBlueprints = new Dictionary<Entity, int>();
            for (int i = 0; i < containerAmount; i++)
            {
                foreach ((Entity card, int amount) in GetBlueprints(random))
                {
                    if (rewardedBlueprints.ContainsKey(card))
                        rewardedBlueprints[card] += amount;
                    else
                        rewardedBlueprints[card] = amount;
                }
            }

            return CreateNewCardsNotifications(rewardedBlueprints);
        }

        private Dictionary<Entity, int> GetBlueprints(Random random)
        {
            int tier3Amount = random.Next(0, 100) <= ContainerInfo.Tier3Probability
                ? random.Next(ContainerInfo.MinTier3Amount, ContainerInfo.MaxTier3Amount)
                : 0;
            int tier2Amount = random.Next(0, 100) <= ContainerInfo.Tier2Probability
                ? random.Next(ContainerInfo.MinTier2Amount, ContainerInfo.MaxTier2Amount)
                : 0;
            int totalBlueprints = tier3Amount + tier2Amount;
            int tier1Amount = random.Next(0, 100) <= ContainerInfo.Tier1Probability
                ? random.Next(ContainerInfo.MinAmount - totalBlueprints, ContainerInfo.MaxAmount - totalBlueprints) : 0;

            return GetRandomBlueprints(tier3Amount, ModuleCards.Tier3Modules, random)
                .Concat(GetRandomBlueprints(tier2Amount, ModuleCards.Tier2Modules, random))
                .Concat(GetRandomBlueprints(tier1Amount, ModuleCards.Tier1Modules, random))
                .ToDictionary(k => k.Key, v => v.Value);
        }

        private Dictionary<Entity, int> GetRandomBlueprints(int amount, List<Entity> possibleBlueprints, Random random)
        {
            Dictionary<Entity, int> selectedBlueprints = new Dictionary<Entity, int>();
            int nextCardProbability = 0;

            while (selectedBlueprints.Sum(x => x.Value) < amount)
            {
                if (random.Next(0, 100) <= nextCardProbability || !selectedBlueprints.Any())
                {
                    Entity blueprint = possibleBlueprints[random.Next(possibleBlueprints.Count)];
                    int count = 1;

                    if (selectedBlueprints.ContainsKey(blueprint))
                         count += selectedBlueprints[blueprint] + 1;
                    selectedBlueprints[blueprint] = count;

                    nextCardProbability = NextCardProbabilityDelta(random);
                }
                else
                {
                    selectedBlueprints[selectedBlueprints.Last().Key]++;
                    nextCardProbability += NextCardProbabilityDelta(random);
                }
            }

            return selectedBlueprints;
        }

        private int NextCardProbabilityDelta(Random random) => random.Next(9, 20);

        private ContainerInfo.ContainerInfo ContainerInfo { get; }
        protected List<Entity> ItemListBlueprints { get; }
    }
}
