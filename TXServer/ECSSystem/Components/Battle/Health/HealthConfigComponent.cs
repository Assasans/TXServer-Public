using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Health
{
    [SerialVersionUID(8420700272384380156)]
    public class HealthConfigComponent : Component
    {
        public HealthConfigComponent(float baseHealth)
        {
            BaseHealth = baseHealth;
        }
        
        public float BaseHealth { get; set; }
    }
}