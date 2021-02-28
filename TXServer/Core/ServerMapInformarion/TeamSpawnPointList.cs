using System.Collections.Generic;

namespace TXServer.Core.ServerMapInformation
{
    public class TeamSpawnPointList
    {
        public IList<SpawnPoint> BlueTeam { get; set; }
        public IList<SpawnPoint> RedTeam { get; set; }
    }
}
