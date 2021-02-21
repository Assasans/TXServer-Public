using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1490937016798L)]
	public class PromoCodeCheckResultEvent : ECSEvent
	{
        public PromoCodeCheckResultEvent(string code, PromoCodeCheckResult result)
        {
            this.Code = code;
            this.Result = result;
        }

        public string Code { get; set; }
		public PromoCodeCheckResult Result { get; set; }
	}
}
