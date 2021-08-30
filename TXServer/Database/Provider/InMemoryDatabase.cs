using Microsoft.EntityFrameworkCore;
using TXServer.Core.Logging;

namespace TXServer.Database.Provider
{
    public class InMemoryDatabase : EntityFrameworkDatabase
    {
        private readonly InMemoryDatabaseConfig _config;

        public InMemoryDatabase(InMemoryDatabaseConfig config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            Logger.Debug($"Connecting to in-memory://{_config.Database}...");

            builder.UseInMemoryDatabase(_config.Database);
        }
    }
}
