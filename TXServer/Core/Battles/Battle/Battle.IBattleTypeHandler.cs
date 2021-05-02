namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public interface IBattleTypeHandler
        {
            Battle Battle { get; init; }

            void SetupBattle();
            void Tick();

            void OnPlayerAdded(BattleTankPlayer battlePlayer);
            void OnPlayerRemoved(BattleTankPlayer battlePlayer);
        }
    }
}
