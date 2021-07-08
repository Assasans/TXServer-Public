using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Item.Module
{
    [SerialVersionUID(636319924231605265)]
    public class ModuleCardsCompositionComponent : Component
    {
        public ModuleCardsCompositionComponent(int tier)
        {
            (CraftPrice, AllPrices) = tier switch
            {
                <= 0 => (new ModulePrice(2), Tier1Prices),
                1 => (new ModulePrice(5), Tier2Prices),
                >= 2 => (new ModulePrice(10), Tier3Prices)
            };

            UpgradePrices = AllPrices.Skip(1).ToList();
        }

        public ModulePrice CraftPrice { get; set; }
        public List<ModulePrice> UpgradePrices { get; set; }

        [ProtocolIgnore] public List<ModulePrice> AllPrices { get; set; }

        private static readonly List<ModulePrice> Tier1Prices = new()
        {
            new ModulePrice(2, 0, 0), new ModulePrice(5, 100, 5), new ModulePrice(10, 640, 10),
            new ModulePrice(50, 1280, 20), new ModulePrice(100, 1600, 26), new ModulePrice(200, 2560, 41),
            new ModulePrice(500, 4160, 67), new ModulePrice(800, 5440, 87), new ModulePrice(1100, 6400, 102),
            new ModulePrice(1400, 22400, 358)
        };
        private static readonly List<ModulePrice> Tier2Prices = new()
        {
            new ModulePrice(5, 0, 0), new ModulePrice(10, 950, 15), new ModulePrice(30, 1920, 31),
            new ModulePrice(80, 3840, 61), new ModulePrice(140, 4800, 77), new ModulePrice(200, 7680, 123),
            new ModulePrice(320, 20800, 333), new ModulePrice(450, 27200, 435), new ModulePrice(600, 32000, 512),
            new ModulePrice(1400, 112000, 1792)
        };
        private static readonly List<ModulePrice> Tier3Prices = new()
        {
            new ModulePrice(10, 0, 0), new ModulePrice(40, 2880, 46), new ModulePrice(80, 3840, 61),
            new ModulePrice(190, 7680, 123), new ModulePrice(250, 9600, 156), new ModulePrice(400, 15360, 246),
            new ModulePrice(500, 41600, 666), new ModulePrice(800, 54400, 870), new ModulePrice(1100, 64000, 1024),
            new ModulePrice(3000, 224000, 3584)
        };
    }
}
