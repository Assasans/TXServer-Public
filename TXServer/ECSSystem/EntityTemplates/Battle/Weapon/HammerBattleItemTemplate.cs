using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.Components.Battle.Weapon.Hammer;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Weapon
{
    [SerialVersionUID(4939169559170921259L)]
    public class HammerBattleItemTemplate : DiscreteWeaponTemplate
    {
        private const string ConfigPath = "garage/weapon/hammer";

        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new HammerBattleItemTemplate(), ConfigPath, tank, battlePlayer);

            entity.Components.UnionWith(new Component[]
            {
                Config.GetComponent<HammerPelletConeComponent>(ConfigPath.Replace("garage", "battle")),
                Config.GetComponent<MagazineWeaponComponent>(ConfigPath),
                new HammerComponent(),
                new MagazineReadyStateComponent(),
            });

            return entity;
        }
    }
}
