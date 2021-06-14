using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents
{
    public static class StreamHitConfig
    {
        public class Periods : RangedComponent, IConvertibleComponent<StreamHitConfigComponent>
        {
            public void Convert(StreamHitConfigComponent component)
            {
                (component.LocalCheckPeriod, component.SendToServerPeriod) = (FinalValue, FinalValue);
            }
        }

        public class DetectStaticHit : BoolComponent, IConvertibleComponent<StreamHitConfigComponent>
        {
            public void Convert(StreamHitConfigComponent component) => component.DetectStaticHit = Value;
        }
    }
}
