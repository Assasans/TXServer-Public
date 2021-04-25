using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Types;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.EntityTemplates.Battle.Bonus;
using System.Text.Json;
using System.IO;
using System.Linq;
using TXServer.ECSSystem.Events.Battle.Bonus;

namespace TXServer.Core.Battles
{
    public class BattleBonus
    {
        public BattleBonus(BonusType bonusType, Bonus bonus, Battle battle)
        {
            BonusType = bonusType;
            Position = bonus.Position;
            HasParachute = bonus.HasParachute;
            State = BonusState.Unused;
            if (!HasParachute) SpawnHeight = 0;
            if (battle.TypeHandler is not Battle.MatchMakingBattleHandler) GoldboxCrystals = 0;
            Battle = battle;
        }

        public void CreateRegion()
        {
            State = BonusState.RegionShared;
            BonusRegion = BonusRegionTemplate.CreateEntity(BonusType, Position);
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(BonusRegion);
        }

        public void CreateBonus(Entity battleEntity)
        {
            if (BonusType != BonusType.GOLD)
                BonusEntity = SupplyBonusTemplate.CreateEntity(BonusType, BonusRegion, new Vector3(Position.X, Position.Y + SpawnHeight, Position.Z), battleEntity);
            else
                BonusEntity = GoldBonusWithCrystalsTemplate.CreateEntity(BonusType, BonusRegion, new Vector3(Position.X, Position.Y + SpawnHeight, Position.Z), battleEntity);
            Battle.MatchPlayers.Select(x => x.Player).ShareEntity(BonusEntity);
            State = BonusState.Spawned;
        }

        public void Take(Player player)
        {
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new BonusTakenEvent(), BonusEntity);
            if (BonusType == BonusType.GOLD)
                Battle.MatchPlayers.Select(x => x.Player).SendEvent(new GoldTakenNotificationEvent(), player.BattlePlayer.MatchPlayer.BattleUser);
            Battle.MatchPlayers.Select(x => x.Player).UnshareEntity(BonusEntity);
            
            State = BonusType == BonusType.GOLD ? BonusState.Unused : BonusState.Redrop;
            player.BattlePlayer.MatchPlayer.UserResult.BonusesTaken += 1;
            
            switch (BonusType)
            {
                case BonusType.GOLD:
                    player.Data.SetCrystals(player.Data.Crystals + CurrentCrystals);
                    break;
                default:
                    _ = new SupplyEffect(BonusType, player.BattlePlayer.MatchPlayer, cheat:false);
                    break;
            }
        }

        public Entity BonusRegion { get; set; }
        public Entity BonusEntity { get; set; }
        public BonusType BonusType { get; }
        private Vector3 Position { get; }
        private int SpawnHeight { get; } = 30;
        private bool HasParachute { get; }
        private Battle Battle { get; }

        private static int GoldboxCrystals { get; set; } = 1000;
        public int CurrentCrystals { get; set; } = GoldboxCrystals;

        public BonusState State
        {
            get => _state;
            set
            {
                _state = value;

                switch (value)
                {
                    case BonusState.RegionShared:
                        if (BonusType == BonusType.GOLD)
                            StateChangeCountdown = 20;
                        break;
                    case BonusState.Redrop:
                        StateChangeCountdown = 180;
                        break;
                    case BonusState.Unused:
                        CurrentCrystals = GoldboxCrystals;
                        break;
                }
            }
        }
        private BonusState _state;
        public double StateChangeCountdown { get; set; } = -1;
    }
}
