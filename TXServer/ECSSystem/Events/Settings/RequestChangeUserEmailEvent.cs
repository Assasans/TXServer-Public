using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using System;
using TXServer.ECSSystem.Components;
using TXServer.Core.Commands;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1457935367814L)]
    public class RequestChangeUserEmailEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            // TODO: save changed email in database, change confirmation status + send confirmation email to new address 
            // TODO: what does subscribed boolean in Component below do, there already is another single UserSubscribeComponent
            CommandManager.SendCommands(player,
                new ComponentChangeCommand(player.User, new ConfirmedUserEmailComponent(Email, false)),
                new SendEventCommand(new EmailVacantEvent(Email), entity));
        }

        public string Email { get; set; }
    }
}
