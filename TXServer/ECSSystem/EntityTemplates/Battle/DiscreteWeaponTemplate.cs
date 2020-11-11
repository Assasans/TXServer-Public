﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-1716200834009238305L)]
    public class DiscreteWeaponTemplate : WeaponTemplate
    {
        protected static new Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank)
        {
            Entity entity = WeaponTemplate.CreateEntity(template, configPath, tank);
            entity.Components.Add(new ImpactComponent(1.3f));
            entity.Components.Add(new KickbackComponent(1.3f));
            entity.Components.Add(new DiscreteWeaponEnergyComponent(1.0f, 1.0f));
            entity.Components.Add(new DiscreteWeaponComponent());

            return entity;
        }
    }
}