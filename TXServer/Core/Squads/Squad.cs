using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Groups;
using TXServer.ECSSystem.Components.Squad;
using TXServer.ECSSystem.EntityTemplates.Chat;
using TXServer.ECSSystem.EntityTemplates.Squad;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core.Squads
{
    public class Squad
    {
        private static readonly ILogger Logger = Log.Logger.ForType<Squad>();

        public Squad(Player leader, Player participant)
        {
            Leader = leader;
            SquadEntity = SquadTemplate.CreateEntity();

            AddPlayer(leader);
            AddPlayer(participant);
        }

        public void AddPlayer(Player player)
        {
            Logger.WithPlayer(player).Information("Joined squad {Squad}", SquadEntity.EntityId);

            player.SquadPlayer = new SquadPlayer(player, Leader == player, this);
            player.SharePlayers(Participants.Select(x => x.Player));
            Participants.SharePlayers(player);

            if (SquadChatEntity == null)
                SquadChatEntity = SquadChatTemplate.CreateEntity(player);
            else
            {
                SquadChatEntity.ChangeComponent<ChatParticipantsComponent>(component =>
                { component.Users.Add(player.User); });
            }
            player.ShareEntities(SquadChatEntity);

            Participants.Add(player.SquadPlayer);
        }

        public void RemovePlayer(Player player, bool disband)
        {
            Logger.WithPlayer(player).Information("Left squad {Squad}", SquadEntity.EntityId);

            Participants.Remove(player.SquadPlayer);
            player.UnsharePlayers(Participants.Select(x => x.Player));
            Participants.UnsharePlayers(player);

            player.UnshareEntities(SquadEntity, SquadChatEntity);
            player.User.RemoveComponent<SquadGroupComponent>();

            SquadChatEntity.ChangeComponent<ChatParticipantsComponent>(component =>
            { component.Users.Remove(player.User); });

            if (player.SquadPlayer.IsLeader)
            {
                player.User.RemoveComponent<SquadLeaderComponent>();
                if (Participants.Count > 1)
                {
                    SquadPlayer newLeader = Participants[new Random().Next(Participants.Count)];
                    newLeader.Player.User.AddComponent(new SquadLeaderComponent());
                    newLeader.IsLeader = true;
                }
            }

            player.SquadPlayer = null;

            if (Participants.Count == 1 && !disband) DisbandSquad();
        }

        private void DisbandSquad()
        {
            foreach (SquadPlayer squadPlayer in Participants.ToArray())
                RemovePlayer(squadPlayer.Player, true);

            Logger.Information("Squad {Squad} was disbanded", SquadEntity.EntityId);
        }

        public void ProcessBattleLeave(Player player, Battle battle)
        {
            if (battle.BattleState is BattleState.Ended) return;

            if (player.IsSquadLeader && ParticipantsWithoutLeader.All(x => x.Player.IsInBattle && !x.Player.IsInMatch))
            {
                foreach (SquadPlayer participant in ParticipantsWithoutLeader)
                    battle.RemovePlayer(participant.Player.BattlePlayer);
            }
            else
            {
                if (player.IsSquadLeader) DisbandSquad();
                else if (Leader.IsInBattle) RemovePlayer(player, false);
            }
        }

        public Entity SquadEntity { get; }
        private Entity SquadChatEntity { get; set; }
        private Player Leader { get; }
        public List<SquadPlayer> Participants { get; } = new();
        public IEnumerable<SquadPlayer> ParticipantsWithoutLeader => Participants.Where(x => !x.IsLeader);
        public static int MaxSquadSize => Config.GetComponent<SquadConfigComponent>("squad").MaxSquadSize;
    }
}
