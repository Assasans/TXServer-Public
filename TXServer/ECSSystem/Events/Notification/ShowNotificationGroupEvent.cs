using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1487160556894L)]
	public class ShowNotificationGroupEvent : ECSEvent
	{
        public ShowNotificationGroupEvent(int expectedMembersCount)
        {
            ExpectedMembersCount = expectedMembersCount;
        }

        public int ExpectedMembersCount { get; set; }
	}
}
