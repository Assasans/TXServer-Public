using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.Database;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1438075609642)]
    public class AutoLoginUserEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData data = player.Server.Database.GetPlayerData(Uid);
            if (data == null)
            {
                player.SendEvent(new UidInvalidEvent(), entity);
                player.SendEvent(new LoginFailedEvent(), entity);
                return;
            }

            if (data.HardwareId == HardwareFingerprint && data.AutoLoginToken != null)
            {
                data.Player = player;
                player.Data = data;

                byte[] token = player.EncryptionComponent.RsaDecrypt(EncryptedToken);
                if (data.AutoLoginToken.SequenceEqual(token))
                {
                    player.LogInWithDatabase(entity);
                    return;
                }
            }

            player.SendEvent(new AutoLoginFailedEvent(), entity);
        }

        public string Uid { get; set; }

        public byte[] EncryptedToken { get; set; }

        public string HardwareFingerprint { get; set; }
    }
}
