﻿using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.PlayerAuth
{
    public struct EmailAvailableRequest : INetSerializable
    {
        public int packetId;
        public string email;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(email);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            email = reader.ReadString();
        }
    }
}
