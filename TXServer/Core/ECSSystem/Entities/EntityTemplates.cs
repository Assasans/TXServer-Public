namespace TXServer.Core.ECSSystem.EntityTemplates
{
    public interface IEntityTemplate { }

    [SerialVersionUID(1429771189777)]
    public class ClientSessionTemplate : IEntityTemplate { }

    [SerialVersionUID(1454928219469)]
    public class LobbyTemplate : IEntityTemplate { }

    [SerialVersionUID(1534913762047)]
    public class FractionsCompetitionTemplate : IEntityTemplate { }

    [SerialVersionUID(1544501900637L)]
    public class FractionTemplate : IEntityTemplate { }
}
