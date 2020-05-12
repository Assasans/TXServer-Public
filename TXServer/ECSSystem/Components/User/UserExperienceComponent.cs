using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(-777019732837383198)]
	public class UserExperienceComponent : Component
	{
		public UserExperienceComponent(long Experience)
		{
			this.Experience = Experience;
		}

		public long Experience { get; set; }
	}
}
