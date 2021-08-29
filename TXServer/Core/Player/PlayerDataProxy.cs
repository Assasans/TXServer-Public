using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXServer.Core
{
    public class PlayerDataProxy : PlayerData
    {
        public PlayerDataProxy(long uid, string username, string hashedPassword, string email, bool emailVerified, string hardwareId, string token) : base(uid)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
            EmailVerified = emailVerified;
            HardwareId = hardwareId;
            AutoLoginToken = token;

            AcceptedFriendIds = new();
            IncomingFriendIds = new();
            OutgoingFriendIds = new();
        }

        public override PlayerData From(object dataReader)
        {
            try
            {
                Email = "none";
                Subscribed = false;
                Username = "tim";
                HashedPassword = "abc";
                CountryCode = "EN";
                Avatar = 6224;
                Admin = true;
                Beta = false;
                Crystals = 1000000;
                XCrystals = 50000;
                Original = (PlayerData)Clone();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return this;
        }

        public void SetSettings(string countryCode, DateTime premiumExpiration, bool subscribed)
        {
            CountryCode = countryCode;

            Avatar = 6224;
            PremiumExpirationDate = premiumExpiration;
            Subscribed = subscribed;
        }

        public void SetStats(long XP, long crystals, long xCrystals, bool isAdmin, bool isBeta)
        {
            Experience = XP;
            Crystals = crystals;
            XCrystals = xCrystals;
            Admin = isAdmin;
            Beta = isBeta;
        }
    }
}
