using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(635950079224407790L)]
    public class ShaftStateConfigComponent : Component
    {
        public ShaftStateConfigComponent(float waitingToActivationTransitionTimeSec, float activationToWorkingTransitionTimeSec, float finishToIdleTransitionTimeSec)
        {
            WaitingToActivationTransitionTimeSec = waitingToActivationTransitionTimeSec;
            ActivationToWorkingTransitionTimeSec = activationToWorkingTransitionTimeSec;
            FinishToIdleTransitionTimeSec = finishToIdleTransitionTimeSec;
        }

        public float WaitingToActivationTransitionTimeSec { get; set; }
        public float ActivationToWorkingTransitionTimeSec { get; set; }
        public float FinishToIdleTransitionTimeSec { get; set; }
    }
}