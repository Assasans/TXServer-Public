using Microsoft.Data.Sqlite;

namespace TXServer.Core.Data.Database.Impl
{
    public sealed class SqLitePlayer : PlayerData
    {
        public SqLitePlayer(string uid) : base(uid) { }

        public override PlayerData From(object dataReader)
        {
            SqliteDataReader data = dataReader as SqliteDataReader;
            
            for (var i = 0; i < data.FieldCount; i++)
            {
                SetValue(data.GetName(i), data.GetValue(i));
            }

            Original = (PlayerData)Clone();
            return this;
        }
    }
}