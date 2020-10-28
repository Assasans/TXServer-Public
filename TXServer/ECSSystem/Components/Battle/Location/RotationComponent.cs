using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Location
{
    [SerialVersionUID(-1853333282151870933)]
    public class RotationComponent : Component
    {
        public RotationComponent(Vector3 rotationEuler)
        {
            RotationEuler = rotationEuler;
        }
        
        public Vector3 RotationEuler { get; set; }
    }
}