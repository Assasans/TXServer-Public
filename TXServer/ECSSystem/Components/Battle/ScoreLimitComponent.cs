using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(-3048295118496552479)]
    public class ScoreLimitComponent : Component
    {
        public ScoreLimitComponent(int scoreLimit)
        {
            ScoreLimit = scoreLimit;
        }
        
        public int ScoreLimit { get; set; }
    }
}