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
                module.Duration = 0;
                module.CooldownDuration = 1000;
            }
            else
            {
                module.ConfigPath = $"garage/module/upgrade/properties/{name.Split('/').Last()}";

                var durationComponent = Config.GetComponent<ModuleEffectDurationPropertyComponent>(module.ConfigPath, false);
                var cooldownComponent = Config.GetComponent<ModuleCooldownPropertyComponent>(module.ConfigPath);

                module.Level = 1;
                if (module is not GoldModule)
                    module.Level = module.ModuleEntity.GetComponent<SlotUserItemInfoComponent>().UpgradeLevel;

                module.Duration = durationComponent?.UpgradeLevel2Values[module.Level - 1] ?? 0;
                module.CooldownDuration = cooldownComponent.UpgradeLevel2Values[module.Level - 1];
            }

            module.Init();

			return module;
		}
	}
}
