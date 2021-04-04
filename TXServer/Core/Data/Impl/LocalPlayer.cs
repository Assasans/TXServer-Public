using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Types.Punishments;

namespace TXServer.Core.Data.Database.Impl
{
    public class LocalPlayer : PlayerData
    {
        public LocalPlayer(string uid) : base(uid) {}

        public override PlayerData From(object dataReader)
        {
            try
            {
                Email = "none";
                Subscribed = false;
                Username = "tim";
                HashedPassword = "abc";
                CountryCode = "EN";
                Avatar = "8b74e6a3-849d-4a8d-a20e-be3c142fd5e8";
                Admin = true;
                Beta = true;
                Crystals = 1000000;
                XCrystals = 50000;
                AcceptedFriendIds = new List<long>();
                IncomingFriendIds = new List<long>();
                OutgoingFriendIds = new List<long>();
                Punishments = new List<Punishment>();
                Original = (PlayerData) Clone();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return this;
        }
    }
}