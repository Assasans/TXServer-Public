namespace TXServer.Core.Battles.Effect
{
    public class ModuleInfo
    {
        public int Level { get; set; }
        public int Cards { get; set; }

        public ModuleInfo(int level, int cards)
        {
            Level = level;
            Cards = cards;
        }
    }
}
