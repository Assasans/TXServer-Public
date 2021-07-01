using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(-9188485263407476652L)]
    public class SelfDestructionComponent : Component
    {
        public void OnAttached(Player player, Entity tank) =>
            player.BattlePlayer.MatchPlayer.SelfDestructionTime = DateTime.UtcNow.AddSeconds(5);
    }
}
