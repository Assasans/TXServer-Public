using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636319924231605265)]
    public class ModuleCardsCompositionComponent : Component
    {
        public ModulePrice CraftPrice { get; set; } = new ModulePrice();

        public List<ModulePrice> UpgradePrices { get; set; } = new List<ModulePrice>();
    }
}
