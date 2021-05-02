﻿using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(3413384256910001471L)]
    public class IsisBattleItemTemplate : StreamWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new IsisBattleItemTemplate(), "battle/weapon/isis", tank, battlePlayer);
            entity.Components.Add(new IsisComponent());

            return entity;
        }
    }
}