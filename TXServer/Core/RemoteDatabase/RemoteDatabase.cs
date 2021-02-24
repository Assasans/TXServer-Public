using System;
using System.Threading.Tasks;
using TXServer.Utils;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

using MySql.Data.MySqlClient;
using System.Net.Sockets;

namespace TXServer.Core.RemoteDatabase
{
    public static class RemoteDatabase
    {
        public static MySqlConnection Socket { get; private set; }

        public static bool isInitilized { get; private set; } = false;
        public static Users Users { get; private set; } = new Users();
        public static bool Initilize(string address, short port, string dbName, string username, string password)
        {
            if (isInitilized) Dispose();
            try
            {
                Socket = new MySqlConnection($"SERVER={address};PORT={port};DATABASE={dbName};UID={username};PASSWORD={password};");
                isInitilized = true;
            }
            catch (Exception err)
            {
                Console.WriteLine($"SQLDatabase.Initilize() failed => '{err}'");
                return false;
            }
            Console.WriteLine("SQL Intiilization Complete");
            return true;
        }

        public static bool OpenConnection()
        {
            if (!isInitilized) throw new Exception("SQL is not initilized! Use the Initilize function!");
            try
            {
                Socket.Open();
                return true;
            }
            catch (MySqlException _error)
            {
                // When handling errors, you can your application's response based on the error number.
                // The two most common error numbers when connecting are as follows:
                // 0: Cannot connect to server.
                // 1045: Invalid user name and/or password.
                switch (_error.Number)
                {
                    case 0:
                        Console.WriteLine("SQLDatabase.Connect() failed! Could not establish a connection to the server");
                        break;
                    case 1045:
                        Console.WriteLine("SQLDatabase.Connect() failed! Invalid Username/Password combination");
                        break;
                    default:
                        Console.WriteLine($"SQLDatabase.Connect() failed! Unknown error code '{_error.Number}'");
                        break;
                }
                return false;
            }
        }

        public static bool CloseConnection()
        {
            try
            {
                Socket.Close();
                return true;
            }
            catch (MySqlException _error)
            {
                Console.WriteLine($"SQLDatabase.Disconnect() failed! Reason: {_error.Message}");
                return false;
            }
        }

        public static void Dispose()
        {
            if (!isInitilized) return;
            isInitilized = false;
            Socket.Dispose();
            Socket = null;
        }
    }
    public class Users
    {
        static string Escape(string value) => MySqlHelper.EscapeString(value);
        public readonly string HashAlg = MD5Util.Compute("tanki-x/users"); // The hash algorithm... I couldn't think of anything so I just made something up

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "I already do filter input via the 'Escape()' function")]
        public async Task<UserDatabaseRow> GetUserByName(string username)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    string.Format(
                        "SELECT * FROM users WHERE username = '{0}'",
                        Escape(username)
                    ),
                    RemoteDatabase.Socket
                );

                MySqlDataReader response = request.ExecuteReader(System.Data.CommandBehavior.SingleRow);

                UserDatabaseRow user = UserDatabaseRow.Empty;
                if (response.HasRows)
                {
                    await response.ReadAsync();
                    user = new UserDatabaseRow
                    {
                        uid = response.GetInt32("uid"),
                        username = response.GetString("username"),
                        hashedPassword = response.GetString("hashedPassword"),
                        email = response.GetString("email"),
                        isEmailVerified = response.GetBoolean("isEmailVerified"),
                        score = response.GetInt64("score"),
                        crystals = response.GetInt64("crystals"),
                        xcrystals = response.GetInt64("xcrystals"),
                        countryCode = response.GetString("countryCode"),
                        avatar = response.GetString("avatar"),
                        isAdmin = response.GetBoolean("isAdmin"),
                        isBeta = response.GetBoolean("isBeta"),
                        isSubscribed = response.GetBoolean("isSubscribed"),
                        premiumExpiration = response.GetDateTime("premiumExpiration")
                    };
                }

                response.Close();
                RemoteDatabase.CloseConnection();

                return user;
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public async Task<UserDatabaseRow> GetUserByEmail(string email)
        {
            if (!StringUtil.isEmailValid(email)) throw new UserError("Invalid Email");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    string.Format(
                        "SELECT * FROM users WHERE email = '{0}'",
                        Escape(email)
                    ),
                    RemoteDatabase.Socket
                );

                MySqlDataReader response = request.ExecuteReader(System.Data.CommandBehavior.SingleRow);

                UserDatabaseRow user = UserDatabaseRow.Empty;
                if (response.HasRows)
                {
                    await response.ReadAsync();
                    user = new UserDatabaseRow
                    {
                        uid = response.GetInt32("uid"),
                        username = response.GetString("username"),
                        hashedPassword = response.GetString("hashedPassword"),
                        email = response.GetString("email"),
                        isEmailVerified = response.GetBoolean("isEmailVerified"),
                        score = response.GetInt64("score"),
                        crystals = response.GetInt64("crystals"),
                        xcrystals = response.GetInt64("xcrystals"),
                        countryCode = response.GetString("countryCode"),
                        avatar = response.GetString("avatar"),
                        isAdmin = response.GetBoolean("isAdmin"),
                        isBeta = response.GetBoolean("isBeta"),
                        isSubscribed = response.GetBoolean("isSubscribed"),
                        premiumExpiration = response.GetDateTime("premiumExpiration")
                    };
                }

                response.Close();
                RemoteDatabase.CloseConnection();

                return user;
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public bool UserExists(string username)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    string.Format(
                        "SELECT username FROM users WHERE username = '{0}'",
                        Escape(username)
                    ),
                    RemoteDatabase.Socket
                );

                MySqlDataReader response = request.ExecuteReader(System.Data.CommandBehavior.SingleRow);

                return response.HasRows;
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public async Task<bool> EmailAvailable(string email)
        {
            if (!StringUtil.isEmailValid(email)) throw new UserError("Invalid Email");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    string.Format(
                        "SELECT email FROM users WHERE email = '{0}'",
                        Escape(email)
                    ),
                    RemoteDatabase.Socket
                );

                MySqlDataReader response = request.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                RemoteDatabase.CloseConnection();

                return response.HasRows;
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public async Task<UserDatabaseRow> Create(UserDatabaseRow user)
        {
            if (user.username == null ||
                user.hashedPassword == null) throw new UserError("Fill out all the fields");
            if (!StringUtil.isUsernameValid(user.username))
                throw new UserError("Invalid username");
            if (UserExists(user.username))
                throw new UserError("Username not Available");

            if (RemoteDatabase.OpenConnection())
            {
                try
                {
                    string _now = DateTime.UtcNow.ToString("yyyy-mm-dd H:mm:ss");
                    Console.WriteLine("CURRENT TIME: " + _now);
                    MySqlCommand request = new MySqlCommand(
                        string.Format(
                            "INSERT INTO users(username, hashedPassword, email) VALUES('{0}', '{1}', '{2}')",
                            Escape(user.username),
                            Escape(user.hashedPassword),
                            Escape(user.email ?? "")
                        ),
                        RemoteDatabase.Socket
                    );
                    Console.WriteLine("FINAL CMD: " + request.CommandText);
                    request.ExecuteNonQuery();

                    RemoteDatabase.CloseConnection();

                    return await GetUserByName(user.username);
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                    RemoteDatabase.CloseConnection();
                    throw new UserError("Something unexpected happened");
                }
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        // Set functions
        public void SetUsername(string username, string newUsername)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET username = '{1}' WHERE username = '{0}'",
                        Escape(username),
                        Escape(newUsername)
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetHashedPassword(string username, string hashedPassword)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET hashedPassword = '{1}' WHERE username = '{0}'",
                        Escape(username),
                        Escape(hashedPassword)
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetEmail(string username, string email)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET email = '{1}' WHERE username = '{0}'",
                        Escape(username),
                        Escape(email)
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetEmailVerified(string username, bool verified)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET isEmailVerified = {1} WHERE username = '{0}'",
                        Escape(username),
                        verified ? "1" : "0"
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetScore(string username, long score)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET score = {1} WHERE username = '{0}'",
                        Escape(username),
                        score
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetCrystals(string username, long crystals)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET crystals = {1} WHERE username = '{0}'",
                        Escape(username),
                        crystals
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetXCrystals(string username, long xcry)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET xcrystals = {1} WHERE username = '{0}'",
                        Escape(username),
                        xcry
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetCountryCode(string username, string countryCode)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET countryCode = '{1}' WHERE username = '{0}'",
                        Escape(username),
                        Escape(countryCode)
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetAvatar(string username, string avatar)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET avatar = '{1}' WHERE username = '{0}'",
                        Escape(username),
                        Escape(avatar)
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetIsAdmin(string username, bool value)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET isAdmin = {1} WHERE username = '{0}'",
                        Escape(username),
                        value ? "1" : "0"
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetIsBeta(string username, bool value)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET isBeta = {1} WHERE username = '{0}'",
                        Escape(username),
                        value ? "1" : "0"
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetIsSubscribed(string username, bool verified)
        {
            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET isSubscribed = {1} WHERE username = '{0}'",
                        Escape(username),
                        verified ? "1" : "0"
                    ),
                    RemoteDatabase.Socket
                );

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }

        public void SetPremiumExpiration(string username, DateTime expiration)
        {

            if (!StringUtil.isUsernameValid(username)) throw new UserError("Invalid Username");

            if (RemoteDatabase.OpenConnection())
            {
                MySqlCommand request = new MySqlCommand(
                    cmdText: string.Format(
                        "UPDATE users SET premiumExpiration = '{1}' WHERE username = '{0}'",
                        Escape(username),
                        expiration.ToString("yyyy-mm-dd H:mm:ss")
                    ),
                    RemoteDatabase.Socket
                ); ;

                request.ExecuteNonQuery();
                RemoteDatabase.CloseConnection();
            }
            else throw new SocketException((int)SocketError.ConnectionRefused);
        }
    }
    public struct UserDatabaseRow
    {
        //static string Escape(string value) => MySqlHelper.EscapeString(value);
        public static readonly UserDatabaseRow Empty = new UserDatabaseRow(); // An empty instance to compare for mistakes
        public static readonly UserDatabaseRow OfflineProfile = new UserDatabaseRow
        {
            hashedPassword = "abc",
            email = "none",
            isEmailVerified = false,
            score = 0,
            crystals = 0,
            xcrystals = 0,
            countryCode = "EN",
            avatar = "8b74e6a3-849d-4a8d-a20e-be3c142fd5e8",
            isAdmin = false,
            isBeta = false,
            isSubscribed = false,
            premiumExpiration = DateTime.Now
        };
        public int uid;
        public string username;
        public string hashedPassword;
        public string email;
        public bool isEmailVerified;
        public long score;
        public long crystals;
        public long xcrystals;
        public bool isSubscribed;
        public string countryCode;
        public string avatar;
        public bool isAdmin;
        public bool isBeta;
        public DateTimeOffset premiumExpiration;
    }
    public class UserInfo : PlayerData
    {
        public UserInfo(string uid) : base(uid) { }

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
    }
}
