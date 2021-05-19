using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core.Database;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1438075609642)]
    public class AutoLoginUserEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (Server.DatabaseNetwork.isReady)
            {
                PacketSorter.GetUserViaUsername(Uid, response =>
                {
                    PlayerData data = new PlayerDataProxy(
                        response.uid,
                        Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.username),
                        Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hashedPassword),
                        Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.email),
                        response.emailVerified,
                        Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareId),
                        Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareToken)
                    );
                    if (data.HardwareId == HardwareFingerprint) {
                        data.Player = player;
                        player.Data = data;

                        // Still need to somehow get the token from the EncryptedToken but idk how, I'll get there soon

                        player.LogInWithDatabase(entity);
                    }
                    else player.SendEvent(new AutoLoginFailedEvent(), entity);
                });
            }
            else
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
        }

        public string Uid { get; set; }

        public byte[] EncryptedToken { get; set; }

        public string HardwareFingerprint { get; set; }
    }
}
