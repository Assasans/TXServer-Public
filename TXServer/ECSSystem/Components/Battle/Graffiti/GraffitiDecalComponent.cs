using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(636100801609006236L)]
    public class GraffitiDecalComponent : Component
    {
        public Vector3 SprayPosition { get; set; }
        public Vector3 SprayDirection { get; set; }
        public Vector3 SprayUpDirection { get; set; }
    }
}