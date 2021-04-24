using System.Linq;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class StubModule : BattleModule {
		public StubModule(MatchPlayer player, Entity garageModule) : base(
			player,
			ModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override void Activate() {
			string name = ModuleEntity.TemplateAccessor.ConfigPath.Split('/').Last();
			
			Player.Player.SendEvent(
				new ChatMessageReceivedEvent() {
					UserId = 1,
					UserUid = "System",
					UserAvatarId = "",
					Message = $"[StubModule] This module is not implemented yet ({name})",
					SystemMessage = true
				},
				Player.Battle.GeneralBattleChatEntity
			);
		}
	}
}
