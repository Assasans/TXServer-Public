using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1503654626834L)]
    public class CurrentSeasonRewardForClientComponent : Component // Corrupted
    {
        public byte /* List<EndSeasonRewardItem> */ Rewards { get; set; } = 0;
    }
}
