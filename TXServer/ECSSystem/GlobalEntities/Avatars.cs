using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Avatars
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Player.GenerateId();

                item.TemplateAccessor.Template = new AvatarUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity AlgheriaFlag { get; } = new Entity(-910231604, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/algheria_flag"),
                new MarketItemGroupComponent(-910231604));
            public Entity Alpha { get; } = new Entity(934969104, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/alpha"),
                new MarketItemGroupComponent(934969104));
            public Entity Antheusfighter { get; } = new Entity(6200, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/antheusfighter"),
                new MarketItemGroupComponent(6200));
            public Entity ArgentinaFlag { get; } = new Entity(-1202231726, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/argentina_flag"),
                new MarketItemGroupComponent(-1202231726));
            public Entity ArmeniaFlag { get; } = new Entity(6201, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/armenia_flag"),
                new MarketItemGroupComponent(6201));
            public Entity AustraliaFlag { get; } = new Entity(-1731159719, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/australia_flag"),
                new MarketItemGroupComponent(-1731159719));
            public Entity AzerbaijanFlag { get; } = new Entity(1174801258, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/azerbaijan_flag"),
                new MarketItemGroupComponent(1174801258));
            public Entity BelorussiaFlag { get; } = new Entity(6202, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/belorussia_flag"),
                new MarketItemGroupComponent(6202));
            public Entity Blackwidow { get; } = new Entity(1286714921, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/blackwidow"),
                new MarketItemGroupComponent(1286714921));
            public Entity Bogatyr { get; } = new Entity(6203, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/bogatyr"),
                new MarketItemGroupComponent(6203));
            public Entity BoyValday { get; } = new Entity(6204, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/boy_valday"),
                new MarketItemGroupComponent(6204));
            public Entity BrazilFlag { get; } = new Entity(6205, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/brazil_flag"),
                new MarketItemGroupComponent(6205));
            public Entity CanadaFlag { get; } = new Entity(2123817583, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/canada_flag"),
                new MarketItemGroupComponent(2123817583));
            public Entity ChinaFlag { get; } = new Entity(1651733218, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/china_flag"),
                new MarketItemGroupComponent(1651733218));
            public Entity Crab { get; } = new Entity(-1632342818, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/crab"),
                new MarketItemGroupComponent(-1632342818));
            public Entity Cyborg { get; } = new Entity(6206, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/cyborg"),
                new MarketItemGroupComponent(6206));
            public Entity CzechFlag { get; } = new Entity(-171184634, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/czech_flag"),
                new MarketItemGroupComponent(-171184634));
            public Entity DarinaLiekhtonien { get; } = new Entity(-1473761734, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/darina_liekhtonien"),
                new MarketItemGroupComponent(-1473761734));
            public Entity DeanCunningham { get; } = new Entity(1479495495, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/dean_cunningham"),
                new MarketItemGroupComponent(1479495495));
            public Entity Dygest { get; } = new Entity(-983105950, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/dygest"),
                new MarketItemGroupComponent(-983105950));
            public Entity Eagle { get; } = new Entity(938326966, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/eagle"),
                new MarketItemGroupComponent(938326966));
            public Entity EgyptFlag { get; } = new Entity(-1089651106, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/egypt_flag"),
                new MarketItemGroupComponent(-1089651106));
            public Entity Elite { get; } = new Entity(6207, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/elite"),
                new MarketItemGroupComponent(6207));
            public Entity EmmaBonney { get; } = new Entity(-2119697962, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/emma_bonney"),
                new MarketItemGroupComponent(-2119697962));
            public Entity EstoniaFlag { get; } = new Entity(1781929660, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/estonia_flag"),
                new MarketItemGroupComponent(1781929660));
            public Entity EvgenyRomanov { get; } = new Entity(-1130044595, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/evgeny_romanov"),
                new MarketItemGroupComponent(-1130044595));
            public Entity FinlandFlag { get; } = new Entity(-241092701, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/finland_flag"),
                new MarketItemGroupComponent(-241092701));
            public Entity FranceFlag { get; } = new Entity(117983554, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/france_flag"),
                new MarketItemGroupComponent(117983554));
            public Entity Frontierfighter { get; } = new Entity(6208, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/frontierfighter"),
                new MarketItemGroupComponent(6208));
            public Entity Gambler { get; } = new Entity(1569342716, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/gambler"),
                new MarketItemGroupComponent(1569342716));
            public Entity GarciaLopez { get; } = new Entity(1953932916, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/garcia_lopez"),
                new MarketItemGroupComponent(1953932916));
            public Entity GeorgiaFlag { get; } = new Entity(6209, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/georgia_flag"),
                new MarketItemGroupComponent(6209));
            public Entity GermanyFlag { get; } = new Entity(6210, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/germany_flag"),
                new MarketItemGroupComponent(6210));
            public Entity Ghostrider { get; } = new Entity(6211, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/ghostrider"),
                new MarketItemGroupComponent(6211));
            public Entity GirlValday { get; } = new Entity(6212, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/girl_valday"),
                new MarketItemGroupComponent(6212));
            public Entity GreatbritainFlag { get; } = new Entity(6213, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/greatbritain_flag"),
                new MarketItemGroupComponent(6213));
            public Entity GreeceFlag { get; } = new Entity(1175725104, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/greece_flag"),
                new MarketItemGroupComponent(1175725104));
            public Entity Heart { get; } = new Entity(941211128, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/heart"),
                new MarketItemGroupComponent(941211128));
            public Entity Hellhound { get; } = new Entity(869046809, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/hellhound"),
                new MarketItemGroupComponent(869046809));
            public Entity HungaryFlag { get; } = new Entity(232090231, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/hungary_flag"),
                new MarketItemGroupComponent(232090231));
            public Entity Hydra { get; } = new Entity(941809812, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/hydra"),
                new MarketItemGroupComponent(941809812));
            public Entity IndiaFlag { get; } = new Entity(-2048808190, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/india_flag"),
                new MarketItemGroupComponent(-2048808190));
            public Entity IrmaMassacre { get; } = new Entity(855079933, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/irma_massacre"),
                new MarketItemGroupComponent(855079933));
            public Entity IsraelFlag { get; } = new Entity(-1042657987, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/israel_flag"),
                new MarketItemGroupComponent(-1042657987));
            public Entity ItalyFlag { get; } = new Entity(-1887453418, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/italy_flag"),
                new MarketItemGroupComponent(-1887453418));
            public Entity Jedi { get; } = new Entity(6214, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/jedi"),
                new MarketItemGroupComponent(6214));
            public Entity KazakhstanFlag { get; } = new Entity(6215, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/kazakhstan_flag"),
                new MarketItemGroupComponent(6215));
            public Entity LatviaFlag { get; } = new Entity(-1036805170, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/latvia_flag"),
                new MarketItemGroupComponent(-1036805170));
            public Entity Legends { get; } = new Entity(1820960568, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/legends"),
                new MarketItemGroupComponent(1820960568));
            public Entity LithuaniaFlag { get; } = new Entity(-657725808, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/lithuania_flag"),
                new MarketItemGroupComponent(-657725808));
            public Entity MagnusGrim { get; } = new Entity(706204327, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/magnus_grim"),
                new MarketItemGroupComponent(706204327));
            public Entity Mammoth { get; } = new Entity(6216, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/mammoth"),
                new MarketItemGroupComponent(6216));
            public Entity MaryNewell { get; } = new Entity(1807734043, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/mary_newell"),
                new MarketItemGroupComponent(1807734043));
            public Entity MaureenLawrie { get; } = new Entity(764921404, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/maureen_lawrie"),
                new MarketItemGroupComponent(764921404));
            public Entity MexicoFlag { get; } = new Entity(224328936, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/mexico_flag"),
                new MarketItemGroupComponent(224328936));
            public Entity MichelleReisner { get; } = new Entity(-2083128946, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/michelle_reisner"),
                new MarketItemGroupComponent(-2083128946));
            public Entity MoldaviaFlag { get; } = new Entity(6217, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/moldavia_flag"),
                new MarketItemGroupComponent(6217));
            public Entity MoroccoFlag { get; } = new Entity(-814801911, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/morocco_flag"),
                new MarketItemGroupComponent(-814801911));
            public Entity NetherlandsFlag { get; } = new Entity(1927964361, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/netherlands_flag"),
                new MarketItemGroupComponent(1927964361));
            public Entity Order { get; } = new Entity(948065536, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/order"),
                new MarketItemGroupComponent(948065536));
            public Entity OskarKhrom { get; } = new Entity(-880569858, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/oskar_khrom"),
                new MarketItemGroupComponent(-880569858));
            public Entity PakistanFlag { get; } = new Entity(287575104, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/pakistan_flag"),
                new MarketItemGroupComponent(287575104));
            public Entity PirateGirl { get; } = new Entity(1455996148, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/pirate_girl"),
                new MarketItemGroupComponent(1455996148));
            public Entity PolandFlag { get; } = new Entity(1472995923, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/poland_flag"),
                new MarketItemGroupComponent(1472995923));
            public Entity Professor { get; } = new Entity(-1330873567, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/professor"),
                new MarketItemGroupComponent(-1330873567));
            public Entity Raccoon { get; } = new Entity(6218, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/raccoon"),
                new MarketItemGroupComponent(6218));
            public Entity RussiaFlag { get; } = new Entity(6219, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/russia_flag"),
                new MarketItemGroupComponent(6219));
            public Entity SaudiArabia { get; } = new Entity(1602513792, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/saudi_arabia_flag"),
                new MarketItemGroupComponent(1602513792));
            public Entity Savage { get; } = new Entity(6220, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/savage"),
                new MarketItemGroupComponent(6220));
            public Entity Skeletoninspace { get; } = new Entity(6221, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/skeletoninspace"),
                new MarketItemGroupComponent(6221));
            public Entity SlovakiaFlag { get; } = new Entity(1776630907, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/slovakia_flag"),
                new MarketItemGroupComponent(1776630907));
            public Entity SloveniaFlag { get; } = new Entity(-224755940, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/slovenia_flag"),
                new MarketItemGroupComponent(-224755940));
            public Entity SpainFlag { get; } = new Entity(399797680, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/spain_flag"),
                new MarketItemGroupComponent(399797680));
            public Entity SpyGirl { get; } = new Entity(1612159053, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/spy_girl"),
                new MarketItemGroupComponent(1612159053));
            public Entity Stormtrooper { get; } = new Entity(6222, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/stormtrooper"),
                new MarketItemGroupComponent(6222));
            public Entity Swatsoldier { get; } = new Entity(6223, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/swatsoldier"),
                new MarketItemGroupComponent(6223));
            public Entity SwedenFlag { get; } = new Entity(394437137, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/sweden_flag"),
                new MarketItemGroupComponent(394437137));
            public Entity Tankist { get; } = new Entity(6224, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/tankist"),
                new MarketItemGroupComponent(6224));
            public Entity Tankix { get; } = new Entity(6225, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/tankix"),
                new MarketItemGroupComponent(6225));
            public Entity TeresaAmsel { get; } = new Entity(1657247215, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/teresa_amsel"),
                new MarketItemGroupComponent(1657247215));
            public Entity ThailandFlag { get; } = new Entity(187162326, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/thailand_flag"),
                new MarketItemGroupComponent(187162326));
            public Entity Troll { get; } = new Entity(6226, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/troll"),
                new MarketItemGroupComponent(6226));
            public Entity TurkeyFlag { get; } = new Entity(893099663, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/turkey_flag"),
                new MarketItemGroupComponent(893099663));
            public Entity UkraineFlag { get; } = new Entity(6227, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/ukraine_flag"),
                new MarketItemGroupComponent(6227));
            public Entity UsaFlag { get; } = new Entity(6228, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/usa_flag"),
                new MarketItemGroupComponent(6228));
            public Entity VeraKlein { get; } = new Entity(-1147853194, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/vera_klein"),
                new MarketItemGroupComponent(-1147853194));
            public Entity Viking { get; } = new Entity(6229, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/viking"),
                new MarketItemGroupComponent(6229));
            public Entity VladimirRepin { get; } = new Entity(-480997961, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/vladimir_repin"),
                new MarketItemGroupComponent(-480997961));
            public Entity Wasp { get; } = new Entity(6230, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/wasp"),
                new MarketItemGroupComponent(6230));
            public Entity YamamotoTsunetomo { get; } = new Entity(605003762, new TemplateAccessor(new AvatarMarketItemTemplate(), "garage/avatar/yamamoto_tsunetomo"),
                new MarketItemGroupComponent(605003762));
        }
    }
}
