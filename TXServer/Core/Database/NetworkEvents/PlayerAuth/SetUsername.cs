﻿using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.PlayerAuth
{
    public struct SetUsername : INetSerializable
    {
        public long uid;
        public string username;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            username = reader.ReadString();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(username);
        }
    }
}
