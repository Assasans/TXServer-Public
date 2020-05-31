using System.Data.SQLite;

namespace TXServer.Core.Data.Database.Impl
{
    public class SqLiteDatabase : IDatabase
    {
        private SQLiteConnection connection;

        public SqLiteDatabase(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
        }

        public PlayerData FetchPlayerData(string uid)
        {
            //todo impl this
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT * FROM Players WHERE UniqueId=@id";
                cmd.Parameters.AddWithValue("@id", uid);
                cmd.Prepare();
                SQLiteDataReader reader = cmd.ExecuteReader();
                
                if (!reader.Read()) return null;
                return new SqLitePlayer(uid).From(reader);
            }
        }

        public bool SavePlayerData(PlayerData data)
        {
            using (var cmd = new SQLiteCommand(connection))
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
            return true;
        }

        public bool Shutdown()
        {
            connection.Close();
            return true;
        }
    }
}