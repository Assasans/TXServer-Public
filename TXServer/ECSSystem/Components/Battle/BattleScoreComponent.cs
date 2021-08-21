using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(1436532217083L)]
    public class BattleScoreComponent : Component
    {
        public BattleScoreComponent(int score, int scoreRed, int scoreBlue)
        {
            Score = score;
            ScoreRed = scoreRed;
            ScoreBlue = scoreBlue;
        }

        public int Score { get; set; }
        public int ScoreRed { get; set; }
        public int ScoreBlue { get; set; }
    }
}
