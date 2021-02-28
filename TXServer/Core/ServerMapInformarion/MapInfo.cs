using System.Collections.Generic;

namespace TXServer.Core.ServerMapInformation
{
    public class MapInfo
    {
        public MapBonusInfo BonusRegions { get; set; }
        public MapFlags Flags { get; set; }
        public IList<PuntativeGeometry> PuntativeGeoms { get; set; }
        public MapSpawnPointInfo SpawnPoints { get; set; }
    }
}
