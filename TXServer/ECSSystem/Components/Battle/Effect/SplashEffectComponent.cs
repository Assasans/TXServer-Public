using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect
{
    [SerialVersionUID(1542363613520L)]
    public class SplashEffectComponent : Component
    {
        public SplashEffectComponent(bool canTargetTeammates)
        {
            CanTargetTeammates = canTargetTeammates;
        }

        public bool CanTargetTeammates { get; set; }
    }
}
