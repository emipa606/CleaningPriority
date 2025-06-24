using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CleaningPriority.UserInterface;

internal class Dialog_CleaningPriority : Window
{
    private const float marginBetweenElements = 6f;
    private const float elementHeight = 24f;
    private const float buttonHeight = 30f;
    private readonly HashSet<Area> addQueue = [];
    private readonly Map map;

    private readonly HashSet<Area> removeQueue = [];

    private Vector2 scrollPos = new(0, 0);

    public Dialog_CleaningPriority(Map currentMap)
    {
        map = currentMap;
        doCloseX = true;
    }

    public override Vector2 InitialSize => new(450f, 400f);

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

        IEnumerable<Area> addable = manager.AddableAreas;
        var playerCanAdd = addable.Any();
        var listRect = new Rect(0f, 0f, inRect.width - 20f,
            manager.AreaCount * (elementHeight + marginBetweenElements));
        var listHolder = new Rect(inRect.x, inRect.y, inRect.width,
            playerCanAdd ? inRect.height - marginBetweenElements - buttonHeight : inRect.height);
        Widgets.BeginScrollView(listHolder, ref scrollPos, listRect);
        var uiLister = new Listing_Standard();
        uiLister.Begin(listRect);
        uiLister.ColumnWidth = listRect.width;
        uiLister.Gap(marginBetweenElements);
        var switchTuple = new Tuple<int, int>(-1, 0);
        for (var i = 0; i < manager.AreaCount; i++)
        {
            var areaIsPriority = manager.PrioritizedArea == manager[i];
            var result = doAreaRow(manager[i], uiLister, manager.AreaCount, i, areaIsPriority);
            switch (result)
            {
                case > 0:
                    switchTuple = new Tuple<int, int>(i, i + 1);
                    break;
                case < 0:
                    switchTuple = new Tuple<int, int>(i, i - 1);
                    break;
            }

            uiLister.Gap(marginBetweenElements);
        }

        if (switchTuple.Item1 != -1)
        {
            manager.ReorderPriorities(switchTuple.Item1, switchTuple.Item2);
        }

        uiLister.End();
        Widgets.EndScrollView();
        if (playerCanAdd)
        {
            doAddRow(listHolder, addable);
        }
    }

    private void doAddRow(Rect listHolderRect, IEnumerable<Area> addableAreas)
    {
        var buttonRect = new Rect(listHolderRect.x,
            listHolderRect.y + listHolderRect.height + marginBetweenElements, listHolderRect.width, buttonHeight);
        TooltipHandler.TipRegion(buttonRect, "AddAreaForPriorityCleaningTip".Translate());
        if (!Widgets.ButtonText(buttonRect, "AddAreaForPriorityCleaning".Translate()))
        {
            return;
        }

        var menu = makeAreasFloatMenu(addableAreas);
        if (menu != null)
        {
            Find.WindowStack.Add(menu);
        }
    }

    private FloatMenu makeAreasFloatMenu(IEnumerable<Area> addableAreas)
    {
        var options = addableAreas.Select(area => new FloatMenuOption(area.Label, delegate { addQueue.Add(area); }))
            .ToList();

        return options.Count > 0 ? new FloatMenu(options) : null;
    }

    private int doAreaRow(Area areaToList, Listing_Standard listing, int count, int priority,
        bool isPriority)
    {
        var rowRect = listing.GetRect(elementHeight);
        var returnValue = 0;

        if (Mouse.IsOver(rowRect))
        {
            GUI.color = areaToList.Color;
            Widgets.DrawHighlightIfMouseover(rowRect);
            GUI.color = Color.white;
        }

        doAreaTooltip(rowRect, count, priority, isPriority);

        var widgetRow = new WidgetRow(rowRect.x, rowRect.y, UIDirection.RightThenUp, rowRect.width);

        if (count > 1 && priority < count - 1)
        {
            if (widgetRow.ButtonIcon(TexButton.ReorderDown))
            {
                returnValue = 1;
            }
        }
        else
        {
            widgetRow.ButtonIcon(TexButton.ReorderDown);
        }

        if (count > 1 && priority > 0)
        {
            if (widgetRow.ButtonIcon(TexButton.ReorderUp))
            {
                returnValue = -1;
            }
        }
        else
        {
            widgetRow.ButtonIcon(TexButton.ReorderUp);
        }

        widgetRow.Icon(areaToList.ColorTexture);
        widgetRow.LabelWithAnchorAndFont(areaToList.Label, -1, TextAnchor.MiddleLeft, GameFont.Small);
        widgetRow.Gap(rowRect.width - (widgetRow.FinalX - rowRect.x) -
                      ((2 * WidgetRow.IconSize) + WidgetRow.DefaultGap));
        if (isPriority)
        {
            widgetRow.Icon(TextureLoader.Clean);
        }
        else
        {
            widgetRow.Gap(WidgetRow.IconSize + WidgetRow.DefaultGap);
        }

        if (count > 1 && widgetRow.ButtonIcon(TextureLoader.Delete))
        {
            removeQueue.Add(areaToList);
        }

        return returnValue;
    }

    private void doAreaTooltip(Rect rowRect, int count, int priority, bool isPrioritized)
    {
        var tooltipString = isPrioritized ? "CleaningAreaIsPrioritized".Translate() : new TaggedString();
        switch (count)
        {
            case 1:
                tooltipString += "OnlyCleaningArea".Translate();
                break;
            case > 1 when priority == 0:
                tooltipString += "CleaningAreaFirst".Translate();
                break;
            case > 1 when priority == count - 1:
                tooltipString += "CleaningAreaLast".Translate();
                break;
            case > 1:
            {
                var manager = map.GetCleaningManager();
                tooltipString +=
                    "CleaningAreaMiddle".Translate(manager[priority - 1].Label, manager[priority + 1].Label);
                break;
            }
        }

        TooltipHandler.TipRegion(rowRect, tooltipString);
    }
}