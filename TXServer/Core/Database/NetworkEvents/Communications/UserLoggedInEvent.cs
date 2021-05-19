using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.Communications {
    public struct UserLoggedInEvent : INetSerializable {
        public long uid;

        public void Serialize(NetWriter writer)
            => writer.Push(uid);

        public void Deserialize(NetReader reader)
            => uid = reader.ReadInt64();
    }
}
