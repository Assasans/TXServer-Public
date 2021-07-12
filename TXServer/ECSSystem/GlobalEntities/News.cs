using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Item.News;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class News
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            // todo: host pictures on yourself & store news items in json
            /*public Entity AllFree { get; } = new Entity(-1837531154,
                new TemplateAccessor(new ConfiguredNewsItemTemplate(), null),
                new NewsItemComponent(headerText:"Everything is free!",
                    previewImageUrl:"https://i.imgur.com/XKNu1OE.png",
                    toolTip:"Everything is free in the current state of the server",
                    layout:NewsItemLayout.MEDIUM),
                new NewsItemSaleLabelComponent("100%"));*/
            public Entity Discord { get; } = new Entity(-1837531150,
                new TemplateAccessor(new ConfiguredNewsItemTemplate(), null),
                new NewsItemComponent(headerText:"Discord Server",
                    previewImageUrl:"https://i.imgur.com/tO633hq.png",
                    centralIconGuid:"d6e6e5e4f34a45d408b589798b80dbe2",
                    toolTip:"Join our official Discord server to chat with the community",
                    externalUrl:"https://discord.gg/revivetx",
                    layout:NewsItemLayout.MEDIUM));
            public Entity Fakers { get; } = new Entity(-1837531151,
                new TemplateAccessor(new ConfiguredNewsItemTemplate(), null),
                new NewsItemComponent(headerText:"Don't fall for fakes!",
                    previewImageUrl:"https://i.imgur.com/I4lhpKZ.png",
                    toolTip:"RTX is free & the only official social network we have is Discord. Don't support fakes!",
                    layout:NewsItemLayout.LARGE));
            public Entity Logo { get; } = new Entity(-1837531152,
                new TemplateAccessor(new ConfiguredNewsItemTemplate(), null),
                new NewsItemComponent(previewImageUrl:"https://i.imgur.com/MFWktJN.png",
                    toolTip:"Thx for playing ^-^",
                    layout:NewsItemLayout.SMALL));
            public Entity VideoUpdate2 { get; } = new Entity(-1837531153,
                new TemplateAccessor(new ConfiguredNewsItemTemplate(), null),
                new NewsItemComponent(new DateTime(2021, 06, 07),
                    headerText:"New update video: modules",
                    previewImageUrl:"https://i.imgur.com/1SXKbBc.jpg",
                    toolTip:"Check out the latest game update video",
                    externalUrl:"https://www.youtube.com/watch?v=CvcdqEmnckE",
                    layout:NewsItemLayout.LARGE));
        }
    }
}
