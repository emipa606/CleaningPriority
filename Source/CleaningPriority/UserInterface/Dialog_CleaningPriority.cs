using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CleaningPriority.UserInterface
{
	class Dialog_CleaningPriority : Window
	{
		private const float marginBetweenElements = 6f;
		private const float elementHeight = 24f;
		private const float buttonHeight = 30f;

		private Vector2 scrollPos = new Vector2(0, 0);
		private readonly Map map;

		private readonly HashSet<Area> removeQueue = new HashSet<Area>();
		private readonly HashSet<Area> addQueue = new HashSet<Area>();

		public override Vector2 InitialSize => new Vector2(450f, 400f);

		public Dialog_CleaningPriority(Map currentMap)
		{
			map = currentMap;
			doCloseX = true;
			forcePause = true;
			closeOnClickedOutside = true;
			absorbInputAroundWindow = true;
		}

		public override void DoWindowContents(Rect inRect)
		{
			var manager = map.GetCleaningManager();
			manager.MarkAllForDraw();
			if (addQueue.Any())
			{
				manager.AddAreaRange(addQueue);
				addQueue.Clear();
			}
			if (removeQueue.Any())
			{
				manager.RemoveAreaRange(removeQueue);
				removeQueue.Clear();
			}
			int reorderableGroup = ReorderableWidget.NewGroup(manager.ReorderPriorities, ReorderableDirection.Vertical, marginBetweenElements);
			IEnumerable<Area> addables = manager.AddableAreas;
			bool playerCanAdd = addables.Any();
			Rect listRect = new Rect(0f, 0f, inRect.width - 20f, manager.AreaCount * (elementHeight + marginBetweenElements));
			Rect listHolder = new Rect(inRect.x, inRect.y, inRect.width, (playerCanAdd) ? inRect.height - marginBetweenElements - buttonHeight : inRect.height);
			Widgets.BeginScrollView(listHolder, ref scrollPos, listRect);
			Listing_Standard uiLister = new Listing_Standard();
			uiLister.Begin(listRect);
			uiLister.ColumnWidth = listRect.width;
			uiLister.Gap(marginBetweenElements);
			for (int i = 0; i < manager.AreaCount; i++)
			{
				bool areaIsPriority = manager.PrioritizedArea == manager[i];
				DoAreaRow(manager[i], uiLister, reorderableGroup, manager.AreaCount, i, areaIsPriority);
				uiLister.Gap(marginBetweenElements);
			}
			uiLister.End();
			Widgets.EndScrollView();
			if (playerCanAdd) DoAddRow(listHolder, addables);
		}

		private void DoAddRow(Rect listHolderRect, IEnumerable<Area> addableAreas)
		{
			Rect buttonRect = new Rect(listHolderRect.x, listHolderRect.y + listHolderRect.height + marginBetweenElements, listHolderRect.width, buttonHeight);
			TooltipHandler.TipRegion(buttonRect, "AddAreaForPriorityCleaningTip".Translate());
			if (Widgets.ButtonText(buttonRect, "AddAreaForPriorityCleaning".Translate()))
			{
				FloatMenu menu = MakeAreasFloatMenu(addableAreas);
				if (menu != null) Find.WindowStack.Add(menu);
			}
		}

		private FloatMenu MakeAreasFloatMenu(IEnumerable<Area> addableAreas)
		{
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (Area area in addableAreas)
			{
				options.Add(new FloatMenuOption(area.Label, delegate ()
				{
					addQueue.Add(area);
				}));
			}
			return (options.Count > 0) ? new FloatMenu(options) : null;
		}

		private void DoAreaRow(Area areaToList, Listing_Standard listing, int group, int count, int priority, bool isPriority)
		{
			Rect rowRect = listing.GetRect(elementHeight);
			ReorderableWidget.Reorderable(group, rowRect);

			if (Mouse.IsOver(rowRect))
			{
				GUI.color = areaToList.Color;
				Widgets.DrawHighlightIfMouseover(rowRect);
				GUI.color = Color.white;
			}

			DoAreaTooltip(rowRect, areaToList, count, priority, isPriority);

			WidgetRow widgetRow = new WidgetRow(rowRect.x, rowRect.y, UIDirection.RightThenUp, rowRect.width);
			widgetRow.Icon(TextureLoader.dragHash);
			widgetRow.Icon(areaToList.ColorTexture, null);
			widgetRow.LabelWithAnchorAndFont(areaToList.Label, -1, TextAnchor.MiddleLeft, GameFont.Small);
			widgetRow.Gap(rowRect.width - (widgetRow.FinalX - rowRect.x) - (2 * WidgetRow.IconSize + WidgetRow.DefaultGap));
			if (isPriority)
			{
				widgetRow.Icon(TextureLoader.clean);
			}
			else
			{
				widgetRow.Gap(WidgetRow.IconSize + WidgetRow.DefaultGap);
			}
			if (count > 1 && widgetRow.ButtonIcon(TextureLoader.delete))
			{
				removeQueue.Add(areaToList);
			}
		}

		private void DoAreaTooltip(Rect rowRect, Area area, int count, int priority, bool isPrioritized)
		{
			TaggedString tooltipString = (isPrioritized) ? "CleaningAreaIsPrioritized".Translate() : new TaggedString();
			if (count == 1)
			{
				tooltipString += "OnlyCleaningArea".Translate();
			}
			else if (count > 1)
			{
				if (priority == 0)
				{
					tooltipString += "CleaningAreaFirst".Translate();
				}
				else if (priority == count - 1)
				{
					tooltipString += "CleaningAreaLast".Translate();
				}
				else
				{
					var manager = map.GetCleaningManager();
					tooltipString += "CleaningAreaMiddle".Translate(manager[priority - 1].Label, manager[priority + 1].Label);
				}
			}
			TooltipHandler.TipRegion(rowRect, tooltipString);
		}
	}
}