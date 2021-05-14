using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Health
{
    [SerialVersionUID(1949198098578360952)]
    public class HealthComponent : Component
    {
        public HealthComponent(float currentHealth, float maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
        
        public float CurrentHealth { get; set; }
        
        public float MaxHealth { get; set; }
    }
}
