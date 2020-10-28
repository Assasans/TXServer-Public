using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(4788927540455272293)]
    public class UserInBattleAsSpectatorComponent : Component
    {
        public UserInBattleAsSpectatorComponent(long battleId)
        {
            BattleId = battleId;
        }
        
        public long BattleId { get; set; }
    }
}