using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Effect
{
    [SerialVersionUID(1432730981932L)]
    public class MineTryExplosionEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Console.WriteLine("tryExplode");
        }
    }
}
