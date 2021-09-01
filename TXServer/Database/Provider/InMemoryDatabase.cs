using Microsoft.EntityFrameworkCore;
using Serilog;
using TXServer.Core.Logging;

namespace TXServer.Database.Provider
{
    public class InMemoryDatabase : EntityFrameworkDatabase
    {
        private static readonly ILogger Logger = Log.Logger.ForType<InMemoryDatabase>();

        private readonly InMemoryDatabaseConfig _config;

        public InMemoryDatabase(InMemoryDatabaseConfig config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            Logger.Debug(
                "Connecting to in-memory://{Database}...",
                _config.Database
            );

            builder.UseInMemoryDatabase(_config.Database);
        }
    }
}
