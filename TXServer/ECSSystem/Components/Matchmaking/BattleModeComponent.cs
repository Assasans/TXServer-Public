using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

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

    public enum BattleMode : byte
    {
        DM,
        TDM,
        CTF,
        CP
    }
}