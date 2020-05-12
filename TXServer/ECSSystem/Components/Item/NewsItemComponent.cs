using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1479374709878L)]
    public class NewsItemComponent : Component
    {
		[OptionalMapped]
		public DateTime? Date { get; set; } = null;

		[OptionalMapped]
		public string HeaderText { get; set; } = "Форум";

		[OptionalMapped]
		public string ShortText { get; set; } = null;

		[OptionalMapped]
		public string LongText { get; set; } = null;

		[OptionalMapped]
		public string PreviewImageUrl { get; set; } = null;

		[OptionalMapped]
		public string PreviewImageGuid { get; set; } = "60c3b3d3e174aa74c90d26593e947b3d";

		[OptionalMapped]
		public string CentralIconGuid { get; set; } = "d6e6e5e4f34a45d408b589798b80dbe2";

		[OptionalMapped]
		public string Tooltip { get; set; } = "Стань частью сообщества и помоги разработчикам обратной связью и предложениями";

		public bool PreviewImageFitInParent { get; set; } = false;

		[OptionalMapped]
		public string ExternalUrl { get; set; } = "http://ru.forum.tankix.com";

		[OptionalMapped]
		public string InternalUrl { get; set; } = null;

		public byte /* enum */ Layout { get; set; } = 0;
    }
}
