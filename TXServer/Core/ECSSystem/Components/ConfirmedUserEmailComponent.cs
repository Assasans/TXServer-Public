﻿namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1457515023113L)]
	public class ConfirmedUserEmailComponent : Component
	{
		public ConfirmedUserEmailComponent(string Email)
		{
			this.Email = Email;
		}

		public string Email { get; set; }

		public bool Subscribed { get; set; }
	}
}
