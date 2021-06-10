using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(-5407563795844501148L)]
    public class StreamHitConfigComponent : Component
    {
        public StreamHitConfigComponent()
        {
            LocalCheckPeriod = 3000;
            SendToServerPeriod = 3000;
            DetectStaticHit = true;
        }

        public float LocalCheckPeriod { get; set; }
        public float SendToServerPeriod { get; set; }
        public bool DetectStaticHit { get; set; }
    }
}
