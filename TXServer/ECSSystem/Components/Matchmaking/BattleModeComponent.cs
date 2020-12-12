using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1432624073184)]
    public class BattleModeComponent : Component
    {
        public BattleModeComponent(BattleMode mode)
        {
            BattleMode = mode;
        }
        
        public BattleMode BattleMode { get; set; }
    }
}