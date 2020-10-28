using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Energy
{
    [SerialVersionUID(8236491228938594733)]
    public class WeaponEnergyComponent : Component
    {
        public WeaponEnergyComponent(float energy)
        {
            Energy = energy;
        }
        
        public float Energy { get; set; }
    }
}