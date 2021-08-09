using System;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Movement
{
    [SerialVersionUID(6959116100408127452)]
    public class MoveCommandEvent : ECSEvent, ISelfEvent
    {
        public void Execute(Player player, Entity tank)
        {
            SelfEvent.Execute(this, player, tank);

            MatchPlayer matchPlayer = player.BattlePlayer.MatchPlayer;
            if (matchPlayer == null || matchPlayer.TankState == TankState.Dead) return;

            Vector3? position = MoveCommand.Movement?.Position;
            if (!position.HasValue) return;

            // Calculate velocity based on 2 previous positions and last position is kept instead
            // Reasons:
            // - velocity sent by client may be corrupted by overflow
            // - latest position may be corrupted too
            Vector3 velocity = matchPlayer.TankPosition - matchPlayer.PrevTankPosition;
            matchPlayer.PrevTankPosition = matchPlayer.TankPosition;
            matchPlayer.TankPosition = position.Value;

            if (MoveCommand.Movement != null)
                matchPlayer.TankQuaternion = (Quaternion) MoveCommand.Movement?.Orientation;

            if (!player.Server.Settings.MapBoundsInactive && PositionFormulas.CheckOverflow(position.Value + velocity))
                matchPlayer.SelfDestructionTime = DateTime.UtcNow;

            if (player.BattlePlayer.Battle.Params.KillZoneEnabled)
            {
                foreach (PuntativeGeometry geometry in player.BattlePlayer.Battle.CurrentMapInfo.PuntativeGeoms)
                {
                    if (PositionFormulas.IsInsideBox(position.Value, geometry.Position, geometry.Size))
                        matchPlayer.SelfDestructionTime = DateTime.UtcNow;
                }
            }
        }

        public IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<MoveCommandServerEvent>();

        public MoveCommand MoveCommand { get; set; }
    }
}
