using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CleaningPriority;

internal class CleaningManager_MapComponent : MapComponent, ICellBoolGiver
{
    private readonly CellBoolDrawer priorityAreasDrawer;

    private List<Area> addableAreas = [];

    private ListerFilthInAreas_MapComponent areaFilthLister;

    private List<Filth> failedFilths = [];
    private bool needToUpdateAddables = true;
    private bool needToUpdatePrioritized = true;

    private Area prioritizedArea;
    private List<Area> priorityList = [];

    public CleaningManager_MapComponent(Map map) : base(map)
    {
        priorityAreasDrawer = new CellBoolDrawer(this, map.Size.x, map.Size.z);
    }

    public int AreaCount => priorityList.Count;

    public Area this[int index] => priorityList[index];

    public bool this[Area area] => priorityList.Contains(area);

    public Area PrioritizedArea
    {
        get
        {
            if (!needToUpdatePrioritized)
            {
                return prioritizedArea;
            }

            ReacalculatePriorityArea();
            needToUpdatePrioritized = false;

            return prioritizedArea;
        }
    }

    public List<Area> PrioritizedAreas => priorityList;

    public List<Area> AddableAreas
    {
        get
        {
            if (!needToUpdateAddables)
            {
                return addableAreas;
            }

            addableAreas = map.areaManager.AllAreas.ToList();
            addableAreas.RemoveAll(x => priorityList.Contains(x));
            needToUpdateAddables = false;

            return addableAreas;
        }
    }

    public Color Color => Color.white;

    public bool GetCellBool(int index)
    {
        foreach (var area in priorityList)
        {
            if (area[index])
            {
                return true;
            }
        }

        return false;
    }

    public Color GetCellExtraColor(int index)
    {
        foreach (var area in priorityList)
        {
            if (area[index])
            {
                return area.Color;
            }
        }

        return Color.clear;
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref priorityList, "RepairingPriority", LookMode.Reference);
        RemoveNullsInList();
        EnsureHasAtLeastOneArea();
    }

    public override void FinalizeInit()
    {
        EnsureHasAtLeastOneArea();
        areaFilthLister = map.GetListerFilthInAreas();
    }

    public override void MapComponentUpdate()
    {
        priorityAreasDrawer.CellBoolDrawerUpdate();
    }

    public bool FilthIsInPriorityAreaSafe(Filth filth)
    {
        return PrioritizedArea != null && PrioritizedArea[filth.Position];
    }

    public IEnumerable<Thing> FilthInCleaningAreas()
    {
        var hashSet = new HashSet<Thing>();
        foreach (var area in priorityList)
        {
            foreach (var filth in areaFilthLister.GetFilthInAreaEnumerator(area))
            {
                hashSet.Add(filth);
            }
        }

        return hashSet;
    }

    public void AddAreaRange(IEnumerable<Area> rangeToAdd)
    {
        foreach (var area in rangeToAdd)
        {
            priorityList.Insert(0, area);
        }

        MarkNeedToRecalculate();
        MarkAddablesOutdated();
    }

    public void RemoveAreaRange(IEnumerable<Area> rangeToRemove)
    {
        priorityList.RemoveAll(rangeToRemove.Contains);
        EnsureHasAtLeastOneArea();
        MarkNeedToRecalculate();
        MarkAddablesOutdated();
    }

    public void ReorderPriorities(int from, int to)
    {
        (priorityList[from], priorityList[to]) = (priorityList[to], priorityList[from]);
        MarkNeedToRecalculate();
    }

    public bool FilthIsInCleaningArea(Filth filth)
    {
        foreach (var area in priorityList)
        {
            if (area[filth.Position])
            {
                return true;
            }
        }

        return false;
    }

    public void OnAreaDeleted(Area deletedArea)
    {
        priorityList.Remove(deletedArea);
        EnsureHasAtLeastOneArea();
        MarkAddablesOutdated();
        MarkNeedToRecalculate();
    }

    public void MarkAddablesOutdated()
    {
        needToUpdateAddables = true;
    }

    public void MarkNeedToRecalculate(Filth filth)
    {
        failedFilths.Add(filth);
        priorityAreasDrawer.SetDirty();
        needToUpdatePrioritized = true;
    }

    public void MarkNeedToRecalculate()
    {
        failedFilths = [];
        priorityAreasDrawer.SetDirty();
        needToUpdatePrioritized = true;
    }

    public void MarkAllForDraw()
    {
        priorityAreasDrawer.MarkForDraw();
    }

    private void RemoveNullsInList()
    {
        priorityList.RemoveAll(x => x == null);
    }

    private void EnsureHasAtLeastOneArea()
    {
        if (!priorityList.Any())
        {
            AddAreaRange(new List<Area> { map.areaManager.Home });
        }
    }

    private void ReacalculatePriorityArea()
    {
        prioritizedArea = null;
        var filthLister = map.GetListerFilthInAreas();
        foreach (var area in priorityList)
        {
            foreach (var thing in filthLister[area])
            {
                var currentFilth = (Filth)thing;
                if (currentFilth.DestroyedOrNull() || failedFilths.Contains(currentFilth) ||
                    currentFilth.TicksSinceThickened < WorkGiver_CleanFilthPrioritized.MinTicksSinceThickened)
                {
                    continue;
                }

                //if (Prefs.DevMode) Log.Message($"Found filth at {currentFilth.Position} : {currentFilth.thickness} : {currentFilth.TicksSinceThickened} : {WorkGiver_CleanFilthPrioritized.MinTicksSinceThickened}");
                prioritizedArea = area;
                return;
            }
        }
    }
}