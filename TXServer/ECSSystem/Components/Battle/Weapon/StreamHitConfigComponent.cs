using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(-5407563795844501148L)]
    public class StreamHitConfigComponent : Component
    {
        public float LocalCheckPeriod { get; set; }
        public float SendToServerPeriod { get; set; }
        public bool DetectStaticHit { get; set; }
    }
}
