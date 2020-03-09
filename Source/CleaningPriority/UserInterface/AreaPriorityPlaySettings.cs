using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CleaningPriority.UserInterface
{
	[HarmonyPatch(typeof(PlaySettings))]
	[HarmonyPatch("DoPlaySettingsGlobalControls")]
	class AreaPriorityPlaySettings
	{
		public static bool showingPrioritySettings = false;

		static void Postfix(WidgetRow row, bool worldView)
		{
			if (worldView) return;
			Rect mouseOverRect = new Rect(row.FinalX - WidgetRow.IconSize, row.FinalY, WidgetRow.IconSize, WidgetRow.IconSize);
			MouseoverSounds.DoRegion(mouseOverRect, SoundDefOf.Mouseover_ButtonToggle);
			if (Mouse.IsOver(mouseOverRect)) Find.CurrentMap.GetCleaningManager().MarkAllForDraw();
			if (row.ButtonIcon(TextureLoader.priorityWindowButton, "OpenCleaningPriorityDialog".Translate()))
			{
				if (!Find.WindowStack.IsOpen<Dialog_CleaningPriority>())
				{
					Find.WindowStack.Add(new Dialog_CleaningPriority(Find.CurrentMap));
				}
				else
				{
					Find.WindowStack.TryRemove(typeof(Dialog_CleaningPriority));
				}
			}
		}
	}
}