using System;
using System.Collections.Generic;
using System.Numerics;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;

namespace TXServer.Core.Battles
{
    public class BattlePlayer
    {
        public static readonly Dictionary<TankState, Type> StateComponents = new Dictionary<TankState, Type>
        {
            //{ TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

        public BattlePlayer(Player player, Entity battleEntity, Entity team)
        {
            Player = player;
            User = player.User;

            BattleUser = BattleUserTemplate.CreateEntity(player, battleEntity, team);
            Team = team;
        }

        public void Reset()
        {
            TankState = TankState.New;
            CollisionsPhase = -1;
            WaitingForTankActivation = false;
            WaitingForExit = false;
        }

        public Player Player { get; }
        public Entity User { get; }
        public Entity Team { get; set; }

        public Entity BattleUser { get; set; }
        public Entity RoundUser { get; set; }

        public Entity Incarnation { get; }
        public Entity Tank { get; set; }
        public Entity Weapon { get; set; }
        public Entity HullSkin { get; set; }
        public Entity WeaponSkin { get; set; }
        public Entity WeaponPaint { get; set; }
        public Entity TankPaint { get; set; }
        public Entity Shell { get; set; }

        public long CollisionsPhase { get; set; } = -1;
        public TankState TankState
        {
            get => _TankState;
            set
            {
                // New state only when tank is deleted or not ready yet
                if (value != TankState.New)
                {
                    if (_TankState == TankState.New)
                    {
                        CommandManager.SendCommandsSafe(Player,
                            new ComponentAddCommand(Tank, (Component)Activator.CreateInstance(StateComponents[value])),
                            new ComponentAddCommand(Tank, new TankMovementComponent(new Movement(new Vector3(0, 2, 0), Vector3.Zero, Vector3.Zero, Quaternion.Identity), new MoveControl(), 0, 0)));
                    }
                    else
                    {
                        CommandManager.SendCommandsSafe(Player,
                            new ComponentAddCommand(Tank, (Component)Activator.CreateInstance(StateComponents[value])),
                            new ComponentRemoveCommand(Tank, StateComponents[_TankState]));
                    }
                }
                _TankState = value;

                switch (value)
                {
                    case TankState.Spawn:
                        TankStateChangeCountdown = 2;
                        break;
                    case TankState.SemiActive:
                        TankStateChangeCountdown = .5;
                        break;
                    case TankState.Dead:
                        TankStateChangeCountdown = 3;
                        break;
                    default:
                        TankStateChangeCountdown = 0;
                        break;
                }
            }
        }
        private TankState _TankState;
        public MoveCommand LastMoveCommand { get; set; }

        public double TankStateChangeCountdown { get; set; }
        public double MatchMakingJoinCountdown { get; set; } = 5;

        public bool WaitingForTankActivation { get; set; }
        public bool WaitingForExit { get; set; }

    }
}
