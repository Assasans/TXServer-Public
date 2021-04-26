using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(4939169559170921259L)]
    public class HammerBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattlePlayer battlePlayer)
        {
            Entity entity = CreateEntity(new HammerBattleItemTemplate(), "battle/weapon/hammer", tank, battlePlayer);
            entity.Components.Add(new HammerPelletConeComponent(15f, 15f, 9));
            entity.Components.Add(new MagazineStorageComponent(1));
            entity.Components.Add(new MagazineWeaponComponent(1, 1f));
            entity.Components.Add(new HammerComponent());
            return entity;
        }
    }
}