using System.Linq;
using TXServer.Core;
using TXServer.Core.Database;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (Server.DatabaseNetwork.IsReady)
            {
                PacketSorter.GetUserViaUsername(Uid, response =>
                    {
                        if (response.uid == -1)
                        {
                            player.SendEvent(new UidInvalidEvent(), entity);
                            player.SendEvent(new LoginFailedEvent(), entity);
                            return;
                        }
                        PlayerData data = new PlayerDataProxy(
                            response.uid,
                            Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.username),
                            Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hashedPassword),
                            Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.email),
                            response.emailVerified,
                            Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareId),
                            Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareToken)
                        );
                        data.Player = player;
                        player.Data = data;

                        player.SendEvent(new PersonalPasscodeEvent(), entity);
                    });
            }
            else
            {
                PlayerData data = Server.Instance.StoredPlayerData.SingleOrDefault(pd => pd.Username == Uid) ??
                                  player.Server.Database.FetchPlayerData(Uid);
                if (data == null) return; // Player#LogIn(Entity) will kick the player
                data.Player = player;
                player.Data = data;
                player.Data.Username = Uid;
                player.SendEvent(new PersonalPasscodeEvent(), entity);
            }
		}

		public string Uid { get; set; }
	}
}
