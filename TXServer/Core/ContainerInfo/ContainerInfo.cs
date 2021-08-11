namespace TXServer.Core.ContainerInfo
{
    public class ContainerInfo
    {
        public string Name { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public int MinTier2Amount { get; set; }
        public int MaxTier2Amount { get; set; }
        public int MinTier3Amount { get; set; }
        public int MaxTier3Amount { get; set; }
        public int Tier1Probability { get; set; }
        public int Tier2Probability { get; set; }
        public int Tier3Probability { get; set; }
    }
}
