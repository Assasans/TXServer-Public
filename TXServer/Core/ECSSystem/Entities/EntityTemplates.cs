namespace TXServer.Core.ECSSystem
{
    public static class EntityTemplates
    {
        public interface IEntityTemplate { }

        [SerialVersionUID(1429771189777)]
        public class ClientSessionTemplate : IEntityTemplate { }

        [SerialVersionUID(1454928219469)]
        public class LobbyTemplate : IEntityTemplate { }
    }
    }
