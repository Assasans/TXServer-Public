using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1435652501758)]
    public class GravityComponent : Component
    {
        public GravityComponent(float gravity, GravityType gravityType)
        {
            Gravity = gravity;
            GravityType = gravityType;
        }
        
        public float Gravity { get; set; }
        
        public GravityType GravityType { get; set; }
    }
}