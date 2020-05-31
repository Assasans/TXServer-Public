using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1438075609642)]
    public class AutoLoginUserEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData data = player.Server.Database.FetchPlayerData(Uid);
            // Player#LogIn(Entity) will kick the player if data == null
            if (data != null)
            {
                data.Player = player;
                player.Data = data;
            }

            player.LogIn(entity);
        }

        public string Uid { get; set; }

        public byte[] EncryptedToken { get; set; }

        public string HardwareFingerprint { get; set; }
    }
}