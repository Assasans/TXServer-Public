using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(4939169559170921259L)]
    public class HammerBattleItemTemplate : DiscreteWeaponTemplate
    {
        private static string _configPath = "garage/weapon/hammer";

        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new HammerBattleItemTemplate(), _configPath, tank, battlePlayer);

            entity.Components.UnionWith(new Component[]
            {
                Config.GetComponent<HammerPelletConeComponent>(_configPath.Replace("garage", "battle")),
                Config.GetComponent<MagazineStorageComponent>(_configPath),
                Config.GetComponent<MagazineWeaponComponent>(_configPath),
                new HammerComponent()
            });

            return entity;
        }
    }
}
