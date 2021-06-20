using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636301946304873717L)]
    public class EmergencyProtectionEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer)
        {
            return CreateEntity(new EmergencyProtectionEffectTemplate(), "/battle/effect/emergencyprotection",
                matchPlayer, addTeam: true);
        }
    }
}
