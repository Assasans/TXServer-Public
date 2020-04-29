using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1502886877871L)]
	public class PresetEquipmentComponent : Component
	{
		public PresetEquipmentComponent()
		{
		}

		public PresetEquipmentComponent(Entity Weapon, Entity Hull)
		{
			this.Weapon = Weapon;
			this.Hull = Hull;
		}

		public Entity Weapon { get; set; } = Weapons.GlobalItems.Smoky;

		public Entity Hull { get; set; } = Hulls.GlobalItems.Hunter;
	}
}
