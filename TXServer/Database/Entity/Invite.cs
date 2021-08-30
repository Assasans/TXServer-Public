using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TXServer.Database.Entity
{
    public class Invite
    {
        public static Invite Create(string code)
        {
            return new Invite()
            {
                Code = code
            };
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int Id { get; set; }

        public string Code { get; set; }
    }
}
