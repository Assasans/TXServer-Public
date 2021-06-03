using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Battles.Effect {
	public class ModuleTypeInfo {
		public ModuleTypeInfo(Type type, TimeSpan cooldown) {
			Type = type;
			Cooldown = cooldown;
		}

		public Type Type { get; }
		public TimeSpan Cooldown { get; }
	}

	public class ModuleRegistry {
		private readonly Dictionary<string, ModuleTypeInfo> name2Module;

		private readonly ModuleTypeInfo stubModule = new(
			typeof(StubModule),
			TimeSpan.FromMilliseconds(1000)
		);

		public ModuleRegistry() {
			name2Module = new Dictionary<string, ModuleTypeInfo>();
		}

		public void Register(string name, ModuleTypeInfo type) {
			name2Module.Add(name, type);
		}

		public ModuleTypeInfo? Get(string name) {
			try {
				return name2Module[name];
			}
			catch {
				return stubModule;
			}
		}

		public BattleModule CreateModule(MatchPlayer player, Entity garageModule) {
			string name = garageModule.TemplateAccessor.ConfigPath;

			ModuleTypeInfo? type = Get(name);
			if(type == null) throw new InvalidOperationException($"Module '{name}' not found");

			BattleModule module = (BattleModule)Activator.CreateInstance(type.Type, player, garageModule);
			module.CooldownDuration = type.Cooldown;

			return module;
		}
	}
}
