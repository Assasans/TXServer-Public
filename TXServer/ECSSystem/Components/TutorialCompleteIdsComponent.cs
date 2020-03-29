using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1505286737090)]
    public class TutorialCompleteIdsComponent : Component
    {
        // Обучение полностью пройдено.
        public List<ulong> CompletedIds { get; set; } = new List<ulong>
        {
            0x00000000190828D4,
            0x000000006C508645,
            0xFFFFFFFFFFF473B3,
            0x000000006C508646,
            0x000000006C508647,
            0x000000001DC04290,
            0x0000000066EFE2F8,
            0xFFFFFFFFD975A705,
            0xFFFFFFFFA0285472,
            0xFFFFFFFFF60C1145,
            0x000000006EC527F6
        };

        public bool TutorialSkipped { get; set; }
    }
}
