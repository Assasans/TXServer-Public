namespace TXServer.Core.ECSSystem.EntityTemplates
{
    public interface IEntityTemplate { }

    [SerialVersionUID(1429771189777)]
    public class ClientSessionTemplate : IEntityTemplate { }

    [SerialVersionUID(1454928219469)]
    public class LobbyTemplate : IEntityTemplate { }

    [SerialVersionUID(1534913762047)]
    public class FractionsCompetitionTemplate : IEntityTemplate { }

    [SerialVersionUID(1544501900637)]
    public class FractionTemplate : IEntityTemplate { }

    [SerialVersionUID(1433752208915)]
    public class UserTemplate : IEntityTemplate { }

    [SerialVersionUID(-5630755063511713066)]
    public class xxTemplate : IEntityTemplate { }
}
