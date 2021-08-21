using System;
using System.Linq;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core.Battles.Effect
{
	public class ModuleRegistry
    {
		private readonly Dictionary<string, Type> _name2Module;

        private readonly Type _stubModule = typeof(StubModule);

		public ModuleRegistry() => _name2Module = new Dictionary<string, Type>();

        public void Register(Dictionary<Entity, Type> modules)
        {
            foreach ((Entity garageModule, Type type) in modules)
                Register(garageModule.TemplateAccessor.ConfigPath, type);
        }

        private void Register(string name, Type type) => _name2Module.Add(name, type);

        private Type Get(string name) => !_name2Module.ContainsKey(name) ? _stubModule : _name2Module[name];

        public BattleModule CreateModule(MatchPlayer matchPlayer, Entity garageModule)
        {
			string name = garageModule.TemplateAccessor.ConfigPath;

			Type type = Get(name);
			if (type == null) throw new InvalidOperationException($"Module '{name}' not found");

			BattleModule module = (BattleModule)Activator.CreateInstance(type, matchPlayer, garageModule);
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
                module.Level = 0;
                if (module is not GoldModule)
                    module.Level = matchPlayer.Player.Data
                        .Modules[garageModule.GetComponent<MarketItemGroupComponent>().Key].Level;
                module.Init();
            }

            return module;
		}
	}
}
