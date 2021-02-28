using System.Collections.Generic;

namespace TXServer.Core.ServerMapInformation
{
    public class MapSpawnPointInfo
    {
        public IList<SpawnPoint> Deathmatch { get; set; }
        public TeamSpawnPointList TeamDeathmatch { get; set; }
        public TeamSpawnPointList CaptureTheFlag { get; set; }
    }
}
