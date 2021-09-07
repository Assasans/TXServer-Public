#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TXServer.Core;
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

        public override PlayerData? GetPlayerData(string username)
        {
            lock (this)
            {
                PlayerData? player = Players.IncludePlayer().SingleOrDefault(player => player.Username == username);
                if (player == null)
                {
                    player = new PlayerData(DateTimeOffset.UtcNow.Ticks);
                    player.InitDefault();
                    player.Username = username;
                    Players.Add(player);
                }

                return player;
            }
        }

        public override PlayerData? GetPlayerDataById(long id)
        {
            lock (this)
            {
                PlayerData? player = Players.IncludePlayer().SingleOrDefault(player => player.UniqueId == id);
                if (player == null)
                {
                    player = new PlayerData(id);
                    player.InitDefault();
                    player.Username = $"InMemory_{id}";
                    Players.Add(player);
                }

                return player;
            }
        }

        [Obsolete("Email replaced with Discord account linking")]
        public override PlayerData? GetPlayerDataByEmail(string email)
        {
            lock (this)
            {
                PlayerData? player = Players.IncludePlayer().SingleOrDefault(player => player.Email == email);
                if (player == null)
                {
                    player = new PlayerData(DateTimeOffset.UtcNow.Ticks);
                    player.InitDefault();
                    player.Username = $"InMemory_{player.UniqueId}";
                    player.Email = email;
                    Players.Add(player);
                }

                return player;
            }
        }
    }
}
