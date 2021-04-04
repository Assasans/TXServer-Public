using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Groups;
using TXServer.ECSSystem.Components.Squad;
using TXServer.ECSSystem.EntityTemplates.Squad;

namespace TXServer.Core.Squads
{
    public class Squad
    {
        public Squad(Player leader, Player participant)
        {
            Leader = leader;
            SquadEntity = SquadTemplate.CreateEntity();

            AddPlayer(leader);
            AddPlayer(participant);
        }

        public void AddPlayer(Player player)
        {
            Logger.Log($"{player}: Joined squad {SquadEntity.EntityId}");

            player.SquadPlayer = new SquadPlayer(player, Leader == player, this);
            player.ShareEntities(Participants.Select(x => x.Player.User));
            Participants.Select(x => x.Player).Where(x => !x.EntityList.Contains(player.User)).ShareEntity(player.User);
            
            Participants.Add(player.SquadPlayer);
        }
        
        public void RemovePlayer(Player player, bool disband)
        {
            Console.WriteLine("remove");
            Logger.Log($"{player}: Left squad {SquadEntity.EntityId}");

            Participants.Remove(player.SquadPlayer);
            player.UnshareEntities(Participants.Where(y => !player.IsInBattleWith(y.Player)).Select(x => x.Player.User));
            Participants.Where(y => !y.Player.IsInBattleWith(player)).Select(x => x.Player).UnshareEntity(player.User);
            
            player.UnshareEntity(SquadEntity);
            player.User.RemoveComponent<SquadGroupComponent>();
            if (player.SquadPlayer.IsLeader) 
                player.User.RemoveComponent<SquadLeaderComponent>();
            player.SquadPlayer = null;

            if (Participants.Count == 1 && !disband) DisbandSquad();
        }

        private void DisbandSquad()
        {
            Console.WriteLine("disband");
            foreach (SquadPlayer squadPlayer in Participants.ToArray())
                RemovePlayer(squadPlayer.Player, true);

            Logger.Log($"Squad {SquadEntity.EntityId} was disbanded");
        }

        public void ProcessBattleLeave(Player player, Battle battle)
        {
            if (battle.BattleState is BattleState.Ended) return;

            if (player.IsSquadLeader && ParticipantsWithoutLeader.All(x => x.Player.IsInBattleLobby && !x.Player.IsInMatch))
            {
                foreach (SquadPlayer participant in ParticipantsWithoutLeader)
                    battle.RemovePlayer(participant.Player.BattlePlayer);
            }
            else
            {
                if (player.IsSquadLeader) DisbandSquad();
                else if (Leader.IsInBattleLobby) RemovePlayer(player, false);
            }
        }
        
        public Entity SquadEntity { get; }
        private Player Leader { get; }
        public List<SquadPlayer> Participants { get; } = new();
        public IEnumerable<SquadPlayer> ParticipantsWithoutLeader => Participants.Where(x => !x.IsLeader);
    }
}