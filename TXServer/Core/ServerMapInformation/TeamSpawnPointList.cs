using System.Collections.Generic;

namespace TXServer.Core.ServerMapInformation
{
    public class TeamSpawnPointList
    {
        public IList<SpawnPoint> RedTeam { get; set; }
        public IList<SpawnPoint> BlueTeam { get; set; }
    }
}
