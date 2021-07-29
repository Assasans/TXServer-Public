namespace TXServer.ECSSystem.ServerComponents.Tank
{
    public static class Speed
    {
        public class SpeedComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedComponent component) => component.Speed = FinalValue;
        }
        public class TurnSpeedComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedComponent component) => component.TurnSpeed = FinalValue;
        }
        public class AccelerationComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.SpeedComponent>
        {
            public void Convert(Components.Battle.Chassis.SpeedComponent component) => component.Acceleration = FinalValue;
        }
    }
}
