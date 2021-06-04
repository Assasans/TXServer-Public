using System;
using System.Linq;
using System.Collections.Generic;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Module;

namespace TXServer.Core.Battles.Effect {
	public class ModuleRegistry {
		private readonly Dictionary<string, Type> name2Module;

        private readonly Type stubModule = typeof(StubModule);

		public ModuleRegistry() {
			name2Module = new Dictionary<string, Type>();
		}

        public void Register(Dictionary<string, Type> modules) {
            foreach ((string name, Type type) in modules)
            {
                Register(name, type);
            }
        }

		public void Register(string name, Type type) {
			name2Module.Add(name, type);
		}

		public Type Get(string name)
        {
            if (!name2Module.ContainsKey(name)) return stubModule;
			return name2Module[name];
		}

		public BattleModule CreateModule(MatchPlayer player, Entity garageModule) {
			string name = garageModule.TemplateAccessor.ConfigPath;

			Type type = Get(name);
			if(type == null) throw new InvalidOperationException($"Module '{name}' not found");

			BattleModule module = (BattleModule)Activator.CreateInstance(type, player, garageModule);

            if (module is StubModule)
            {
                module.Duration = TimeSpan.Zero;
                module.CooldownDuration = TimeSpan.FromMilliseconds(1000);
            }
            else
            {
                string upgradePath = $"garage/module/upgrade/properties/{name.Split('/').Last()}";

                var durationComponent = Config.GetComponent<ModuleEffectDurationPropertyComponent>(upgradePath, false);
                var cooldownComponent = Config.GetComponent<ModuleCooldownPropertyComponent>(upgradePath);

                int level = 1;
                if (module is not GoldModule)
                {
                    level = module.ModuleEntity.GetComponent<SlotUserItemInfoComponent>().UpgradeLevel;
                }

                TimeSpan duration = durationComponent != null ? TimeSpan.FromMilliseconds(durationComponent.UpgradeLevel2Values[level - 1]) : TimeSpan.Zero;
                TimeSpan cooldown = TimeSpan.FromMilliseconds(cooldownComponent.UpgradeLevel2Values[level - 1]);

                module.Duration = duration;
			    module.CooldownDuration = cooldown;
            }

			return module;
		}
	}
}
