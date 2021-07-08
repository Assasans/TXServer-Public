namespace TXServer.ECSSystem.Types
{
	public class ModulePrice
    {
        public ModulePrice(int cards, int crystals = 0, int xCrystals = 0)
        {
            Cards = cards;
            Crystals = crystals;
            XCrystals = xCrystals;
        }

        public int Cards { get; set; } = 10;
        public int Crystals { get; set; } = 200;
        public int XCrystals { get; set; } = 5000;
    }
}
