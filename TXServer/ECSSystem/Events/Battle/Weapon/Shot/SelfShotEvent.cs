using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Shot
{
    [SerialVersionUID(5440037691022467911L)]
    public class SelfShotEvent : ShotEvent, ISelfEvent
    {
        public void Execute(Player player, Entity tank)
        {
            SelfEvent.Execute(this, player, tank);
            player.BattlePlayer.MatchPlayer.TryDeactivateInvisibility();
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteShotEvent>();
    }
}
