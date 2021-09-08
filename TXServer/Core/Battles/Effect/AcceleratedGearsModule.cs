using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Module.AcceleratedGears;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class AcceleratedGearsModule : BattleModule
    {
        public AcceleratedGearsModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectEntity != null) return;

            MatchPlayer.TankEntity.ChangeComponent<SpeedComponent>(component => component.TurnSpeed *= HullRotation);
            MatchPlayer.WeaponEntity.ChangeComponent<WeaponRotationComponent>(component =>
            {
                component.Acceleration *= TurretAcceleration;
                component.Speed *= TurretSpeed;
                MatchPlayer.Weapon.OriginalWeaponRotationComponent = (WeaponRotationComponent) component.Clone();
            });

            EffectEntity = AcceleratedGearsEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;

            MatchPlayer.TankEntity.ChangeComponent<SpeedComponent>(component => component.TurnSpeed /= HullRotation);
            MatchPlayer.WeaponEntity.ChangeComponent<WeaponRotationComponent>(component =>
            {
                component.Acceleration /= TurretAcceleration;
                component.Speed /= TurretSpeed;
                MatchPlayer.Weapon.OriginalWeaponRotationComponent = (WeaponRotationComponent) component.Clone();
            });

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();

            ActivateOnTankSpawn = true;
            AlwaysActiveExceptEmp = true;
            DeactivateOnTankDisable = false;

            HullRotation = Config.GetComponent<ModuleAcceleratedGearsEffectHullRotationSpeedPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TurretAcceleration = Config.GetComponent<ModuleAcceleratedGearsEffectTurretAccelerationPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TurretSpeed = Config.GetComponent<ModuleAcceleratedGearsEffectTurretSpeedPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        private float HullRotation { get; set; }
        private float TurretAcceleration { get; set; }
        private float TurretSpeed { get; set; }
    }
}
