namespace TXServer.ECSSystem.ServerComponents
{
    public static class SpeedConfig
    {
        public class TurnAccelerationComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedConfigComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedConfigComponent component) => component.TurnAcceleration = FinalValue;
        }
        public class SideAccelerationComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedConfigComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedConfigComponent component) => component.SideAcceleration = FinalValue;
        }
        public class ReverseAccelerationComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedConfigComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedConfigComponent component) => component.ReverseAcceleration = FinalValue;
        }
        public class ReverseTurnAccelerationComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedConfigComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedConfigComponent component) => component.ReverseTurnAcceleration = FinalValue;
        }
    }
}
