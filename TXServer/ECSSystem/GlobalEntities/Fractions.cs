using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Fractions
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Competition { get; } = new Entity(1529751975, new TemplateAccessor(new FractionsCompetitionTemplate(), "fractionscompetition"),
                new FinishedFractionsCompetitionComponent());
            public Entity Antaeus { get; } = new Entity(-1650120701, new TemplateAccessor(new FractionTemplate(), "fractionscompetition/fractions/antaeus"),
                new FractionGroupComponent(-1650120701),
                new FractionComponent("antaeus"));
            public Entity Frontier { get; } = new Entity(-372139284, new TemplateAccessor(new FractionTemplate(), "fractionscompetition/fractions/frontier"),
                new FractionGroupComponent(-372139284),
                new FractionComponent("frontier"));
        }
    }
}
