#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TXServer.Database.Entity
{
    public class BlockedUsername
    {
        public static BlockedUsername Create(string username, string? reason = null)
        {
            return new BlockedUsername()
            {
                Username = username,
                Reason = reason
            };
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int Id { get; set; }

        public string Username { get; set; }
        public string? Reason { get; set; }
    }
}
