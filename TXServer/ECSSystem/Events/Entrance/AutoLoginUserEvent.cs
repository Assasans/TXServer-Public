using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1438075609642)]
    public class AutoLoginUserEvent : ECSEvent
    {
        public void Execute(Entity clientSession)
        {
            Player.Instance.Uid = Uid;

            new LoginByPasswordEvent().Execute(clientSession);
        }

        public string Uid { get; set; }

        public byte[] EncryptedToken { get; set; }

        public string HardwareFingerprint { get; set; }
    }
}