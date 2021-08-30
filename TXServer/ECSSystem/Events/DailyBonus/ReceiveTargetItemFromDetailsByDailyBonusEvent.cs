using System.Linq;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.DailyBonus
{
    [SerialVersionUID(636464215401773226L)]
    public class ReceiveTargetItemFromDetailsByDailyBonusEvent : ECSEvent
    {
        public void Execute(Player player, Entity user)
        {
            Entity detailUserItem = player.EntityList.SingleOrDefault(e =>
                e.TemplateAccessor.Template is DetailUserItemTemplate &&
                e.GetComponent<MarketItemGroupComponent>().Key == DetailMarketItemId);
            if (detailUserItem is null) return;
            DetailItemComponent detailComponent =
                Config.GetComponent<DetailItemComponent>(detailUserItem.TemplateAccessor.ConfigPath);

            if (!player.Data.Shards.ContainsId(DetailMarketItemId) ||
                player.Data.Shards.GetById(DetailMarketItemId).Amount < detailComponent.RequiredCount) return;

            // update
            player.Data.Shards.GetById(DetailMarketItemId).Amount -= (int) detailComponent.RequiredCount;
            detailUserItem.ChangeComponent<UserItemCounterComponent>(component =>
                component.Count = player.Data.Shards.GetById(DetailMarketItemId).Amount);

            Entity containerMarketItem =
                player.EntityList.Single(e => e.EntityId == detailComponent.TargetMarketItemId);
            ResourceManager.SaveNewMarketItem(player, containerMarketItem, 1);

            player.SendEvent(new TargetItemFromDailyBonusReceivedEvent(DetailMarketItemId), player.User);
        }

        public long DetailMarketItemId { get; set; }
    }
}
