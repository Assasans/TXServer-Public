using System.Linq;
using Microsoft.EntityFrameworkCore;
using TXServer.Core;
using Z.EntityFramework.Plus;

namespace TXServer.Database
{
    /// <remarks>
    ///   Do not use <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}"/> and
    ///   <see cref="EntityFrameworkQueryableExtensions.Include{TEntity}(IQueryable{TEntity}, string)"/> because of massive overhead,
    ///   which could result in database server out of memory error
    /// </remarks>
    public static class DbSetExtensions
    {
        public static IQueryable<TPlayer> IncludePlayer<TPlayer>(this IQueryable<TPlayer> queryable) where TPlayer : PlayerData
        {
            return queryable
                .IncludeOptimized(player => player.Relations)
                .IncludeOptimized(player => player.Punishments)
                .IncludeOptimized(player => player.Avatars)
                .IncludeOptimized(player => player.Containers)
                .IncludeOptimized(player => player.Covers)
                .IncludeOptimized(player => player.Paints)
                .IncludeOptimized(player => player.Graffiti)
                .IncludeOptimized(player => player.Hulls)
                .IncludeOptimized(player => player.Hulls.Select(hull => hull.Skins))
                .IncludeOptimized(player => player.Weapons)
                .IncludeOptimized(player => player.Weapons.Select(weapon => weapon.Skins))
                .IncludeOptimized(player => player.Weapons.Select(weapon => weapon.ShellSkins))
                .IncludeOptimized(player => player.Modules)
                .IncludeOptimized(player => player.Shards)
                .IncludeOptimized(player => player.Statistics)
                .IncludeOptimized(player => player.DailyBonusReceivedRewards)
                .IncludeOptimized(player => player.CompletedTutorials);
        }
    }
}
