using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.Security {
    public struct LoginSuccessEvent : INetSerializable {
        public void Serialize(NetWriter writer) {}
        public void Deserialize(NetReader reader) {}
    }
}
