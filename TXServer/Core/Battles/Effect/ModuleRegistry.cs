using System;
using System.Linq;
using System.Collections.Generic;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.Battles.Effect {
	public class ModuleRegistry {
		private readonly Dictionary<string, Type> _name2Module;

        private readonly Type _stubModule = typeof(StubModule);

		public ModuleRegistry() {
			_name2Module = new Dictionary<string, Type>();
		}

        public void Register(Dictionary<string, Type> modules) {
            foreach ((string name, Type type) in modules)
            {
                Register(name, type);
            }
        }

        private void Register(string name, Type type) {
			_name2Module.Add(name, type);
		}

        private Type Get(string name) => !_name2Module.ContainsKey(name) ? _stubModule : _name2Module[name];

        public BattleModule CreateModule(MatchPlayer player, Entity garageModule)
        {
			string name = garageModule.TemplateAccessor.ConfigPath;

			Type type = Get(name);
			if (type == null) throw new InvalidOperationException($"Module '{name}' not found");

			BattleModule module = (BattleModule)Activator.CreateInstance(type, player, garageModule);
            module.ModuleType = garageModule.GetComponent<ModuleBehaviourTypeComponent>().Type;
            module.MarketItem = Modules.GlobalItems.GetAllItems().Single(m =>
                m.TemplateAccessor.ConfigPath.Split("/").Last() == name.Split("/").Last());

            if (module is StubModule)
            {
                module.Duration = 0;
                module.CooldownDuration = 1000;
            }
            else
            {
                module.ConfigPath = $"garage/module/upgrade/properties/{name.Split('/').Last()}";
                module.Level = 1;
                if (module is not GoldModule)
                    module.Level = module.ModuleEntity.GetComponent<SlotUserItemInfoComponent>().UpgradeLevel;
                module.Init();
            }

            return module;
		}
	}
}
