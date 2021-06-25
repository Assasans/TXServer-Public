﻿using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Module.Turbospeed;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class TurboSpeedModule : BattleModule
    {
        public TurboSpeedModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            float duration = IsSupply || IsCheat ? 30000 : Duration;
            if (EffectIsActive)
            {
                ChangeDuration(duration);
                return;
            }

            EffectEntity = TurboSpeedEffectTemplate.CreateEntity(MatchPlayer, (long)duration);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component =>
            {
                _prevSpeedComponent = (SpeedComponent)component.Clone();
                component.Speed *= Factor();
            });

            Schedule(TimeSpan.FromMilliseconds(duration), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null ) return;
            if (IsCheat && !DeactivateCheat)
            {
                ChangeDuration(30000);
                return;
            }

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            MatchPlayer.Tank.ChangeComponent(_prevSpeedComponent);

            EffectEntity = null;
            IsCheat = false;
        }

        public override void Init()
        {
            base.Init();

            ModuleFactor = Config.GetComponent<ModuleTurbospeedEffectPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        private float Factor()
        {
            if (IsCheat) return float.MaxValue;
            return IsSupply ? 1.5f : ModuleFactor;
        }
        private float ModuleFactor { get; set; }
        private SpeedComponent _prevSpeedComponent;
    }
}