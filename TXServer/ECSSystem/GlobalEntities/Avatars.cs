using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Avatars : ItemList
    {
        public static Avatars GlobalItems { get; } = new Avatars();

        public static Avatars GetUserItems(Entity user)
        {
            Avatars items = FormatterServices.GetUninitializedObject(typeof(Avatars)) as Avatars;

            foreach (PropertyInfo info in typeof(Avatars).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Algheriaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-910231604, "algheria_flag");
        public Entity Alpha { get; private set; } = AvatarMarketItemTemplate.CreateEntity(934969104, "alpha");
        public Entity Antheusfighter { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6200, "antheusfighter");
        public Entity Argentinaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1202231726, "argentina_flag");
        public Entity Armeniaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6201, "armenia_flag");
        public Entity Australiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1731159719, "australia_flag");
        public Entity Azerbaijanflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1174801258, "azerbaijan_flag");
        public Entity Belorussiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6202, "belorussia_flag");
        public Entity Blackwidow { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1286714921, "blackwidow");
        public Entity Bogatyr { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6203, "bogatyr");
        public Entity Boyvalday { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6204, "boy_valday");
        public Entity Brazilflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6205, "brazil_flag");
        public Entity Canadaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(2123817583, "canada_flag");
        public Entity Chinaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1651733218, "china_flag");
        public Entity Crab { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1632342818, "crab");
        public Entity Cyborg { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6206, "cyborg");
        public Entity Czechflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-171184634, "czech_flag");
        public Entity Darinaliekhtonien { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1473761734, "darina_liekhtonien");
        public Entity Deancunningham { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1479495495, "dean_cunningham");
        public Entity Dygest { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-983105950, "dygest");
        public Entity Eagle { get; private set; } = AvatarMarketItemTemplate.CreateEntity(938326966, "eagle");
        public Entity Egyptflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1089651106, "egypt_flag");
        public Entity Elite { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6207, "elite");
        public Entity Emmabonney { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-2119697962, "emma_bonney");
        public Entity Estoniaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1781929660, "estonia_flag");
        public Entity Evgenyromanov { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1130044595, "evgeny_romanov");
        public Entity Finlandflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-241092701, "finland_flag");
        public Entity Franceflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(117983554, "france_flag");
        public Entity Frontierfighter { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6208, "frontierfighter");
        public Entity Gambler { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1569342716, "gambler");
        public Entity Garcialopez { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1953932916, "garcia_lopez");
        public Entity Georgiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6209, "georgia_flag");
        public Entity Germanyflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6210, "germany_flag");
        public Entity Ghostrider { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6211, "ghostrider");
        public Entity Girlvalday { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6212, "girl_valday");
        public Entity Greatbritainflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6213, "greatbritain_flag");
        public Entity Greeceflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1175725104, "greece_flag");
        public Entity Heart { get; private set; } = AvatarMarketItemTemplate.CreateEntity(941211128, "heart");
        public Entity Hellhound { get; private set; } = AvatarMarketItemTemplate.CreateEntity(869046809, "hellhound");
        public Entity Hungaryflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(232090231, "hungary_flag");
        public Entity Hydra { get; private set; } = AvatarMarketItemTemplate.CreateEntity(941809812, "hydra");
        public Entity Indiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-2048808190, "india_flag");
        public Entity Irmamassacre { get; private set; } = AvatarMarketItemTemplate.CreateEntity(855079933, "irma_massacre");
        public Entity Israelflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1042657987, "israel_flag");
        public Entity Italyflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1887453418, "italy_flag");
        public Entity Jedi { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6214, "jedi");
        public Entity Kazakhstanflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6215, "kazakhstan_flag");
        public Entity Latviaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1036805170, "latvia_flag");
        public Entity Legends { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1820960568, "legends");
        public Entity Lithuaniaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-657725808, "lithuania_flag");
        public Entity Magnusgrim { get; private set; } = AvatarMarketItemTemplate.CreateEntity(706204327, "magnus_grim");
        public Entity Mammoth { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6216, "mammoth");
        public Entity Marynewell { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1807734043, "mary_newell");
        public Entity Maureenlawrie { get; private set; } = AvatarMarketItemTemplate.CreateEntity(764921404, "maureen_lawrie");
        public Entity Mexicoflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(224328936, "mexico_flag");
        public Entity Michellereisner { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-2083128946, "michelle_reisner");
        public Entity Moldaviaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6217, "moldavia_flag");
        public Entity Moroccoflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-814801911, "morocco_flag");
        public Entity Netherlandsflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1927964361, "netherlands_flag");
        public Entity Order { get; private set; } = AvatarMarketItemTemplate.CreateEntity(948065536, "order");
        public Entity Oskarkhrom { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-880569858, "oskar_khrom");
        public Entity Pakistanflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(287575104, "pakistan_flag");
        public Entity Pirategirl { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1455996148, "pirate_girl");
        public Entity Polandflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1472995923, "poland_flag");
        public Entity Professor { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1330873567, "professor");
        public Entity Raccoon { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6218, "raccoon");
        public Entity Russiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6219, "russia_flag");
        public Entity Saudiarabiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1602513792, "saudi_arabia_flag");
        public Entity Savage { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6220, "savage");
        public Entity Skeletoninspace { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6221, "skeletoninspace");
        public Entity Slovakiaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1776630907, "slovakia_flag");
        public Entity Sloveniaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-224755940, "slovenia_flag");
        public Entity Spainflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(399797680, "spain_flag");
        public Entity Spygirl { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1612159053, "spy_girl");
        public Entity Stormtrooper { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6222, "stormtrooper");
        public Entity Swatsoldier { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6223, "swatsoldier");
        public Entity Swedenflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(394437137, "sweden_flag");
        public Entity Tankist { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6224, "tankist");
        public Entity Tankix { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6225, "tankix");
        public Entity Teresaamsel { get; private set; } = AvatarMarketItemTemplate.CreateEntity(1657247215, "teresa_amsel");
        public Entity Thailandflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(187162326, "thailand_flag");
        public Entity Troll { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6226, "troll");
        public Entity Turkeyflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(893099663, "turkey_flag");
        public Entity Ukraineflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6227, "ukraine_flag");
        public Entity Usaflag { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6228, "usa_flag");
        public Entity Veraklein { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-1147853194, "vera_klein");
        public Entity Viking { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6229, "viking");
        public Entity Vladimirrepin { get; private set; } = AvatarMarketItemTemplate.CreateEntity(-480997961, "vladimir_repin");
        public Entity Wasp { get; private set; } = AvatarMarketItemTemplate.CreateEntity(6230, "wasp");
        public Entity Yamamototsunetomo { get; private set; } = AvatarMarketItemTemplate.CreateEntity(605003762, "yamamoto_tsunetomo");
    }
}
