using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using System;
using TXServer.ECSSystem.Components;
using TXServer.Core.Commands;
using TXServer.Core.RemoteDatabase;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1457935367814L)]
    public class RequestChangeUserEmailEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (RemoteDatabase.isInitilized)
            {
                RemoteDatabase.Users.SetEmail(player.tempRow.username, Email);
                RemoteDatabase.Users.SetEmailVerified(player.tempRow.username, false);
            }
            player.tempRow.email = Email;
            player.tempRow.isEmailVerified = false;
            // TODO: Send email to recipient to confirm email
            // TODO: what does subscribed boolean in Component below do, there already is another single UserSubscribeComponent
            CommandManager.SendCommands(player,
                new ComponentChangeCommand(player.User, new ConfirmedUserEmailComponent(Email, false)),
                new SendEventCommand(new EmailVacantEvent(Email), entity));
        }

        public string Email { get; set; }
    }
}
