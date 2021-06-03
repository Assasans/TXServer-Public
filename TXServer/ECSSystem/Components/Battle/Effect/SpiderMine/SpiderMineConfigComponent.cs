﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.SpiderMine {
    [SerialVersionUID(1487227856805L)]
    public class SpiderMineConfigComponent : Component
    {
        public SpiderMineConfigComponent()
        {
            Speed = 8;
            Acceleration = 20;
            Energy = 100;
            IdleEnergyDrainRate = 0;
            ChasingEnergyDrainRate = 0;
        }

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float Energy { get; set; }
        public float IdleEnergyDrainRate { get; set; }
        public float ChasingEnergyDrainRate { get; set; }
    }
}
