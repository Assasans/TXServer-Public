namespace TXServer.ECSSystem.Types
{
	public class ModuleInfo
	{
        public ModuleInfo(long moduleId, long upgradeLevel)
        {
            ModuleId = moduleId;
            UpgradeLevel = upgradeLevel;
        }

        public long ModuleId { get; set; }
		public long UpgradeLevel { get; set; }
    }
}
