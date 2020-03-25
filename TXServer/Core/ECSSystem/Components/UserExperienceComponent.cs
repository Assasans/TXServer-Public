namespace TXServer.Core.ECSSystem.Components
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
