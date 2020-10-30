using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Incarnation
{
    [SerialVersionUID(1491549293967)]
    public class TankIncarnationKillStatisticsComponent : Component
    {
        public int Kills { get; set; }
    }
}