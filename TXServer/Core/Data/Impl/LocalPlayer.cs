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
                Admin = true;
                Beta = true;
                Mod = true;

                Crystals = 1000000;
                XCrystals = 50000;
                Experience = 0;
                PremiumExpirationDate = DateTime.MinValue;

                DailyBonusCycle = 0;
                DailyBonusNextReceiveDate = DateTime.UtcNow;
                DailyBonusReceivedRewards = new List<long>();
                DailyBonusZone = 0;

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

                Avatar = 6224;
                Avatars = new List<long> {6224};
                Containers = new Dictionary<long, int>();
                Covers = new List<long> {-172249613};
                Graffities = new List<long> {1001404575};
                Hulls = new List<long> {537781597};
                HullSkins = new List<long> {1589207088};
                Modules = new Dictionary<long, (int, int)>();
                Paints = new List<long> {-20020438};
                Shells = new List<long> {-966935184, 807172229, 357929046, 48235025, 1067800943, 1322064226, 70311513,
                    530945311, -1408603862, 139800007, 366763244};
                Weapons = new List<long> {-2005747272};
                WeaponSkins = new List<long> {2008385753};

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
