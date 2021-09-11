using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TXServer.Core;

namespace TXServer.Database.Entity
{
    public class Invite
    {
        public static Invite Create(string code, int? accountLimit = null)
        {
            return new Invite()
            {
                Code = code,
                AccountLimit = accountLimit
            };
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int Id { get; set; }

        public string Code { get; set; }

        public int? AccountLimit { get; set; }

        public virtual List<PlayerData> Players { get; set; }
    }
}
