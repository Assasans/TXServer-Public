using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.PlayerSettings
{
    public struct SetCountryCode : INetSerializable
    {
        public long uid;
        public string countryCode;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            countryCode = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(countryCode);
        }
    }
}
