using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public class RanksExperiencesConfigComponent : Component
    {
        public List<int> RanksExperiences { get; set; }
    }
}
