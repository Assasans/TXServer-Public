using Microsoft.Data.Sqlite;

namespace TXServer.Core.Data.Database.Impl
{
    public class SqLiteDatabase : IDatabase
    {
        private SqliteConnection connection;

        public SqLiteDatabase(string connectionString)
        {
            connection = new SqliteConnection(connectionString);
        }

        public PlayerData FetchPlayerData(string uid)
        {
            //todo impl this
            using (var cmd = new SqliteCommand(connection.ConnectionString))
            {
                cmd.CommandText = "SELECT * FROM Players WHERE UniqueId=@id";
                cmd.Parameters.AddWithValue("@id", uid);
                cmd.Prepare();
                SqliteDataReader reader = cmd.ExecuteReader();
                
                if (!reader.Read()) return null;
                return new SqLitePlayer(uid).From(reader);
            }
        }

        public PlayerData FetchPlayerDataByEmail(string email)
        {
            using (var cmd = new SqliteCommand(connection.ConnectionString))
            {
                cmd.CommandText = "SELECT * FROM Players WHERE Email=@email";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Prepare();
                SqliteDataReader reader = cmd.ExecuteReader();

                if (!reader.Read()) return null;
                return new SqLitePlayer(reader.GetString(reader.GetOrdinal("UniqueId"))).From(reader);
            }
        }

        public bool SavePlayerData(PlayerData data)
        {
            using (var cmd = new SqliteCommand(connection.ConnectionString))
            {
                cmd.CommandText = @"UPDATE Players SET ";
                cmd.Parameters.AddWithValue("@key", data.UniqueId);
                cmd.Prepare();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public bool Startup()
        {
            connection.Open();
            //todo create if not exists
            return true;
        }

        public bool Shutdown()
        {
            connection.Close();
            return true;
        }
    }
}