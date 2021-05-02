using TXServer.ECSSystem.Base;

namespace TXServer.Core.Battles
{
    public abstract class BaseBattlePlayer
    {
        public virtual void Reset()
        {
            WaitingForExit = false;
        }

        public Battle Battle { get; protected init; }

        public Player Player { get; protected init; }
        public Entity User { get; protected init; }

        public bool WaitingForExit { get; set; }
        public bool Rejoin { get; set; }
    }
}
