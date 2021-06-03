using System;
using System.Threading;
using System.Threading.Tasks;

using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle {
	[SerialVersionUID(1486015564167L)]
	public class ActivateModuleEvent : ECSEvent {
		public void Execute(Player player, Entity entity) {
			BattleModule? module = player.BattlePlayer.MatchPlayer.Modules.Find((module) => module.ModuleEntity == entity);
			if(module == null) return;

			if(!module.IsEnabled) return;

			if(module.IsOnCooldown) {
				/*
				player.SendEvent(
					new ChatMessageReceivedEvent() {
						UserId = 1,
						UserUid = "System",
						UserAvatarId = "",
						Message = $"Module '{module.GetType().Name}' is on cooldown, stopping...",
						SystemMessage = true
					},
					player.BattlePlayer.MatchPlayer.Battle.GeneralBattleChatEntity
				);
				*/
				return;
			}
			module.StartCooldown();
			
			/*
			player.SendEvent(
				new ChatMessageReceivedEvent() {
					UserId = 1,
					UserUid = "System",
					UserAvatarId = "",
					Message = $"Invoking '{module.GetType().Name}' handler...",
					SystemMessage = true
				},
				player.BattlePlayer.MatchPlayer.Battle.GeneralBattleChatEntity
			);
			*/
			module.Activate();
		}

		public int ClientTime { get; set; }
	}
}
