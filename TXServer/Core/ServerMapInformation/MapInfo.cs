using System.Collections.Generic;

namespace TXServer.Core.ServerMapInformation
{
    public class MapInfo
    {
        public string Name { get; set; }
        public long MapId { get; set; }
        public int MaxPlayers { get; set; }
        public bool MatchMaking { get; set; }
        public bool Custom { get; set; }
        public float GoldProbability { get; set; }
        public MapFlags Flags { get; set; }
        public IList<PuntativeGeometry> PuntativeGeoms { get; set; }
        public MapSpawnPointInfo SpawnPoints { get; set; }
        public IList<TeleportPoint> TeleportPoints { get; set; }
        public MapBonusInfo BonusRegions { get; set; }
    }
}
