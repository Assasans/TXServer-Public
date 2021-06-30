using System;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486041253393L)]
    public class EffectBaseTemplate : IEntityTemplate
    {
        protected static Entity CreateEntity(EffectBaseTemplate template, string configPath,
            MatchPlayer matchPlayer, long durationMs = 0, bool addTeam = false)
        {
            Entity effect = new(new TemplateAccessor(template, configPath),
                new EffectComponent(),
                matchPlayer.Tank.GetComponent<TankGroupComponent>());

            if (durationMs > 0)
            {
                effect.AddComponent(new DurationConfigComponent(durationMs));
                effect.AddComponent(new DurationComponent(DateTime.UtcNow));
            }

            if (addTeam && matchPlayer.Battle.ModeHandler is Core.Battles.Battle.TeamBattleHandler)
            {
                effect.AddComponent(matchPlayer.Player.BattlePlayer.Team.GetComponent<TeamColorComponent>());
                effect.AddComponent(matchPlayer.Player.BattlePlayer.Team.GetComponent<TeamGroupComponent>());
            }

            return effect;
        }
    }
}
