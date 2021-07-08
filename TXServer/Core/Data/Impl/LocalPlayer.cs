using System;
using System.Collections.Generic;
using TXServer.Core.ChatCommands;
using TXServer.Core.Logging;
using TXServer.ECSSystem.GlobalEntities;

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
                Username = "Tanker";
                HashedPassword = "abc";
                RememberMe = true;
                CountryCode = "EN";
                Avatar = "8b74e6a3-849d-4a8d-a20e-be3c142fd5e8";
                Admin = true;
                Beta = true;
                Mod = true;

                Crystals = 1000000;
                XCrystals = 50000;
                Experience = 0;
                PremiumExpirationDate = DateTime.MinValue;

                Reputation = 100;
                League = Leagues.GlobalItems.Training;
                LeagueChestScore = 0;

                AcceptedFriendIds = new List<long>();
                IncomingFriendIds = new List<long>();
                OutgoingFriendIds = new List<long>();
                BlockedPlayerIds = new List<long>();
                ReportedPlayerIds = new List<long>();

                Punishments = new List<Punishment>();
                CompletedTutorialIds = new List<ulong>();

                Modules = new Dictionary<long, (int, int)>();

                Original = (PlayerData) Clone();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return this;
        }
    }
}
