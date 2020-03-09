using UnityEngine;
using Verse;

namespace CleaningPriority.UserInterface
{
	static class CleaningPriorityDrawUtility
	{
		public static void LabelWithAnchorAndFont(this WidgetRow row, string label, float width, TextAnchor anchor, GameFont font)
		{
			Text.Anchor = anchor;
			Text.Font = font;
			row.Label(label, width);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}