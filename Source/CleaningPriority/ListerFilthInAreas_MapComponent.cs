using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CleaningPriority;

internal class ListerFilthInAreas_MapComponent(Map map) : MapComponent(map)
{
    private readonly Dictionary<Area, List<Thing>> filthDictionary = new Dictionary<Area, List<Thing>>();

    public List<Thing> this[Area area]
    {
        get
        {
            EnsureAreaHasKey(area);
            return filthDictionary[area];
        }
    }

    public override void FinalizeInit()
    {
        foreach (var area in map.areaManager.AllAreas)
        {
            EnsureAreaHasKey(area);
        }

        RegenerateDictionary();
    }

    public IEnumerable<Thing> GetFilthInAreaEnumerator(Area area)
    {
        if (!filthDictionary.ContainsKey(area))
        {
            yield break;
        }

        for (var i = 0; i < filthDictionary[area].Count; i++)
        {
            yield return filthDictionary[area][i];
        }
    }

    public void OnFilthSpawned(Filth spawned)
    {
        foreach (var area in filthDictionary.Keys.ToList())
        {
            if (area is Area_Home)
            {
                filthDictionary[area] = map.listerFilthInHomeArea.FilthInHomeArea;
            }
            else if (area[spawned.Position])
            {
                filthDictionary[area].Add(spawned);
            }
        }
    }

    public void OnFilthDespawned(Filth despawned)
    {
        foreach (var area in filthDictionary.Keys.ToList())
        {
            if (area is Area_Home)
            {
                filthDictionary[area] = map.listerFilthInHomeArea.FilthInHomeArea;
            }
            else
            {
                filthDictionary[area].Remove(despawned);
            }
        }
    }

    public void OnAreaChange(IntVec3 cell, bool newVal, Area area)
    {
        if (!area[cell])
        {
            return;
        }

        EnsureAreaHasKey(area);
        var thingsInCell = cell.GetThingList(map);
        if (newVal)
        {
            filthDictionary[area].AddRange(thingsInCell.Where(x => x is Filth));
        }
        else
        {
            filthDictionary[area].RemoveAll(x => thingsInCell.Contains(x));
        }
    }

    public void OnAreaDeleted(Area deletedArea)
    {
        if (filthDictionary.ContainsKey(deletedArea))
        {
            filthDictionary.Remove(deletedArea);
        }
    }


    public void EnsureAreaHasKey(Area area)
    {
        if (!filthDictionary.ContainsKey(area))
        {
            filthDictionary[area] = [];
        }
    }

    private void RegenerateDictionary()
    {
        foreach (var cell in map.AllCells)
        {
            foreach (var area in filthDictionary.Keys)
            {
                OnAreaChange(cell, true, area);
            }
        }
    }
}