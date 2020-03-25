﻿namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(7453043498913563889)]
    public sealed class UserGroupComponent : Component
    {
        public static ulong ComponentSerialUID { get; } = 7453043498913563889;
        public long Key { get; set; } = Player.GenerateId(); // TODO
    }
}
