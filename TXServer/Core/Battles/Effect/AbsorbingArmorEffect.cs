using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Armor;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
	public class AbsorbingArmorEffect : BattleModule
    {
		public AbsorbingArmorEffect(MatchPlayer matchPlayer, Entity garageModule) : base(
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

            EffectEntity = ArmorEffectTemplate.CreateEntity(MatchPlayer, (long)duration);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(duration), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            if (IsCheat && !DeactivateCheat)
            {
                ChangeDuration(30000);
                return;
            }

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EffectEntity = null;
            IsCheat = false;
        }

        public override void Init()
        {
            base.Init();

            ModuleFactor = Config.GetComponent<ModuleArmorEffectPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }


        public float Factor()
        {
            if (IsCheat) return 0;
            return IsSupply ? 0.5f : ModuleFactor;
        }
        private float ModuleFactor { get; set; }

    }
}
