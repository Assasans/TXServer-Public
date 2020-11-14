﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-6419489500262573655L)]
    public class RailgunBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new RailgunBattleItemTemplate(), "battle/weapon/railgun", tank);
            entity.Components.Add(new RailgunChargingWeaponComponent(1f));
            entity.Components.Add(new RailgunComponent());

            return entity;
        }
    }
}