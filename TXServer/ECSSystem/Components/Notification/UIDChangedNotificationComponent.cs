using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1475754429807L)]
    public class UIDChangedNotificationComponent : Component
    {
        // warning: Alternativa logic: oldUserUID = newUserUID 
        public UIDChangedNotificationComponent(string oldUserUID)
        {
            OldUserUID = oldUserUID;
        }

        public string OldUserUID { get; set; }
    }
}
