using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Mine {
    [SerialVersionUID(1431927384785L)]
    public class MineConfigComponent : Component
    {
        public MineConfigComponent()
        {
            DamageFrom = 400;
            DamageTo = 500;
            ActivationTime = 1;
            Impact = 10;
            BeginHideDistance = 10;
            HideRange = 10;
        }

        public float DamageFrom { get; set; }
        public float DamageTo { get; set; }
        public long ActivationTime { get; set; }
        public float Impact { get; set; }
        public float BeginHideDistance { get; set; }
        public float HideRange { get; set; }
    }
}
