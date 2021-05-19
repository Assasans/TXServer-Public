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
                Avatar = "8b74e6a3-849d-4a8d-a20e-be3c142fd5e8";
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

        public void SetSettings(string countryCode, string avatar, DateTime premiumExpiration, bool subscribed)
        {
            CountryCode = countryCode;
            Avatar = avatar;
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
