namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(-5477085396086342998)]
	public class UserUidComponent : Component
	{
		public UserUidComponent(string Uid)
		{
			this.Uid = Uid;
		}

		public string Uid { get; set; }
	}
}
