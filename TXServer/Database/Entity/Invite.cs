using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TXServer.Database.Entity
{
    public class Invite
    {
        public static Invite Create(string code, string username = null)
        {
            return new Invite()
            {
                Code = code,
                Username = username
            };
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int Id { get; set; }

        public string Code { get; set; }

        /// <remarks>If value is <see langword="null" /> - any username is allowed</remarks>
        public string Username { get; set; }
    }
}
