using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    public abstract class WeaponUserItemTemplate : UserItemTemplate, IMountableItemTemplate
	{
		public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            IEntityTemplate template = null;

            switch (marketItem.TemplateAccessor.Template)
            {
                case FlamethrowerMarketItemTemplate _:
                    template = new FlamethrowerUserItemTemplate();
                    break;
                case FreezeMarketItemTemplate _:
                    template = new FreezeUserItemTemplate();
                    break;
                case HammerMarketItemTemplate _:
                    template = new HammerUserItemTemplate();
                    break;
                case IsisMarketItemTemplate _:
                    template = new IsisUserItemTemplate();
                    break;
                case RailgunMarketItemTemplate _:
                    template = new RailgunUserItemTemplate();
                    break;
                case RicochetMarketItemTemplate _:
                    template = new RicochetUserItemTemplate();
                    break;
                case ShaftMarketItemTemplate _:
                    template = new ShaftUserItemTemplate();
                    break;
                case SmokyMarketItemTemplate _:
                    template = new SmokyUserItemTemplate();
                    break;
                case ThunderMarketItemTemplate _:
                    template = new ThunderUserItemTemplate();
                    break;
                case TwinsMarketItemTemplate _:
                    template = new TwinsUserItemTemplate();
                    break;
                case VulcanMarketItemTemplate _:
                    template = new VulcanUserItemTemplate();
                    break;
            }

            Entity userItem = new Entity(new TemplateAccessor(template, marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
            userItem.Components.UnionWith(new Component[]
            {
                new UserGroupComponent(user),
                new ExperienceItemComponent(),
                new UpgradeLevelItemComponent(),
                new UpgradeMaxLevelItemComponent()
            });

            AddToUserItems(typeof(Weapons), userItem);
            return userItem;
        }
	}
}
