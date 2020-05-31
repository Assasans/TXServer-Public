using System.Data.SQLite;

namespace TXServer.Core.Data.Database.Impl
{
    public sealed class SqLitePlayer : PlayerData
    {
        public SqLitePlayer(string uid) : base(uid) { }

        public override PlayerData From(object dataReader)
        {
            SQLiteDataReader data = dataReader as SQLiteDataReader;
            
            for (var i = 0; i < data.FieldCount; i++)
            {
                SetValue(data.GetName(i), data.GetValue(i));
            }

            Original = (PlayerData) Clone();
            return this;
        }
    }
}