using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-3936735916503799349L)]
    public class VulcanBattleItemTemplate : WeaponTemplate
    {
        private static readonly string _configPath = "garage/weapon/vulcan";

        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new VulcanBattleItemTemplate(), _configPath, tank, battlePlayer);

            entity.Components.UnionWith(new Component[]
            {
                Config.GetComponent<VulcanWeaponComponent>(_configPath),
                Config.GetComponent<KickbackComponent>(_configPath),
                Config.GetComponent<ImpactComponent>(_configPath),
                Config.GetComponent<StreamHitConfigComponent>("battle/weapon/vulcan"),
                new VulcanComponent(),
                new StreamWeaponComponent()
            });

            return entity;
        }
    }
}
