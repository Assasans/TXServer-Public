using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using TXServer.Core.Logging;

namespace TXServer.Database.Provider
{
    public class MySqlDatabase : EntityFrameworkDatabase
    {
        private readonly DatabaseConfig _config;

        public MySqlDatabase(DatabaseConfig config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder
            {
                Server = _config.Host,
                Port = (uint) _config.Port,
                UserID = _config.Username,
                Password = _config.Password,
                Database = _config.Database
            };

            Logger.Debug($"Connecting to mysql://{_config.Username}@{_config.Host}:{_config.Port}/{_config.Database}...");

            builder.UseMySql(
                new MySqlConnection(stringBuilder.ToString()),
                new MariaDbServerVersion(Version.Parse(_config.Version))
            );
        }
    }
}
