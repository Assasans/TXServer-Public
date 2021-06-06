﻿using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.Effect
{
    public class TurboSpeedModule : BattleModule
    {
        public TurboSpeedModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        )
        { }

        public override void Activate() =>
            _ = new SupplyEffect(BonusType.SPEED, MatchPlayer, duration: (long)Duration, bonus: false);
    }
}