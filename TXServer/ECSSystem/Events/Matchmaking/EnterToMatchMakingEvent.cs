using System;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Energy;
using TXServer.ECSSystem.Components.Battle.Hull;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Location;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.MatchMaking
{
    [SerialVersionUID(1494937115182)]
    public class EnterToMatchMakingEvent : ECSEvent
    {
        public void Execute(Player player, Entity mode)
        {
            // Validate given mode entity
            // (I don't think chis check is really necessary)
            bool modeValid = false;
            foreach (Entity existingMode in MatchmakingModes.GlobalItems.GetAllItems())
            {
                if (mode.EntityId == existingMode.EntityId)
                {
                    modeValid = true;
                    break;
                }
            }
            if (!modeValid) throw new ArgumentException($"Invalid mode entity: {mode.TemplateAccessor.Template.GetType().Name} ({mode.TemplateAccessor.ConfigPath})");

            Console.WriteLine("Yay, we found a match. " + mode.EntityId);

            CommandManager.SendCommands(player,
                new SendEventCommand(new EnteredToMatchMakingEvent(), mode));

            ServerConnection.GlobalBattle.AddPlayer(player);

            //todo MatchMakingLobbySystem & MatchMakingEntranceSystem.
            // how does setStarting for example work (Lobby)
        }
    }
}