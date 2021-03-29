using System.Collections.Generic;

namespace TXServer.Core.ServerMapInformation
{
    public class BonusList
    {
        public IList<Bonus> Repair { get; set; }
        public IList<Bonus> Armor { get; set; }
        public IList<Bonus> Damage { get; set; }
        public IList<Bonus> Speed { get; set; }
        public IList<Bonus> Gold { get; set; }
    }
}
