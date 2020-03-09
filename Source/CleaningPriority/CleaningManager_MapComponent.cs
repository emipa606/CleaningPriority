using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CleaningPriority
{
	class CleaningManager_MapComponent : MapComponent, ICellBoolGiver
	{
		private List<Area> priorityList = new List<Area>();

		private List<Area> addableAreas = new List<Area>();
		private bool needToUpdateAddables = true;

		private Area prioritizedArea;
		private bool needToUpdatePrioritized = true;

		private ListerFilthInAreas_MapComponent areaFilthLister;
		private CellBoolDrawer priorityAreasDrawer;

		public int AreaCount => priorityList.Count;

		public Area this[int index] => priorityList[index];

		public bool this[Area area] => priorityList.Contains(area);

		public Area PrioritizedArea
		{
			get
			{
				if (needToUpdatePrioritized)
				{
					ReacalculatePriorityArea();
					needToUpdatePrioritized = false;
				}
				return prioritizedArea;
			}
		}

		public List<Area> AddableAreas
		{
			get
			{
				if (needToUpdateAddables)
				{
					addableAreas = map.areaManager.AllAreas.ToList();
					addableAreas.RemoveAll(x => priorityList.Contains(x));
					needToUpdateAddables = false;
				}
				return addableAreas;
			}
		}

		public Color Color => Color.white;

		public CleaningManager_MapComponent(Map map) : base(map)
		{
			priorityAreasDrawer = new CellBoolDrawer(this, map.Size.x, map.Size.z);
		}

		public override void ExposeData()
		{
			Scribe_Collections.Look(ref priorityList, "cleaningPriority", LookMode.Reference);
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
			return (PrioritizedArea != null) && (PrioritizedArea[filth.Position]);
		}

		public IEnumerable<Thing> FilthInCleaningAreas()
		{
			HashSet<Thing> hashSet = new HashSet<Thing>();
			for (int i = 0; i < priorityList.Count; i++)
			{
				foreach (Thing filth in areaFilthLister.GetFilthInAreaEnumerator(priorityList[i]))
				{
					hashSet.Add(filth);
				}
			}
			return hashSet;
		}

		public void AddAreaRange(IEnumerable<Area> rangeToAdd)
		{
			foreach (Area area in rangeToAdd) priorityList.Insert(0, area);
			MarkNeedToRecalculate();
			MarkAddablesOutdated();
		}

		public void RemoveAreaRange(IEnumerable<Area> rangeToRemove)
		{
			priorityList.RemoveAll(x => rangeToRemove.Contains(x));
			EnsureHasAtLeastOneArea();
			MarkNeedToRecalculate();
			MarkAddablesOutdated();
		}

		public void ReorderPriorities(int from, int to)
		{
			Area areaToReorder = priorityList[from];
			priorityList.RemoveAt(from);
			priorityList.Insert(Mathf.Max(0, (to < from) ? to : to - 1), areaToReorder);
			MarkNeedToRecalculate();
		}

		public bool FilthIsInCleaningArea(Filth filth)
		{
			for (int i = 0; i < priorityList.Count; i++)
			{
				if (priorityList[i][filth.Position]) return true;
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

		public void MarkNeedToRecalculate()
		{
			priorityAreasDrawer.SetDirty();
			needToUpdatePrioritized = true;
		}

		public void MarkAllForDraw()
		{
			priorityAreasDrawer.MarkForDraw();
		}

		public bool GetCellBool(int index)
		{
			for (int i = 0; i < priorityList.Count; i++)
			{
				if (priorityList[i][index]) return true;
			}
			return false;
		}

		public Color GetCellExtraColor(int index)
		{
			for (int i = 0; i < priorityList.Count; i++)
			{
				if (priorityList[i][index]) return priorityList[i].Color;
			}
			return Color.clear;
		}

		private void RemoveNullsInList()
		{
			priorityList.RemoveAll(x => x == null);
		}

		private void EnsureHasAtLeastOneArea()
		{
			if (!priorityList.Any()) AddAreaRange(new List<Area>() { map.areaManager.Home });
		}

		private void ReacalculatePriorityArea()
		{
			prioritizedArea = null;
			ListerFilthInAreas_MapComponent filthLister = map.GetListerFilthInAreas();
			for (int i = 0; i < priorityList.Count; i++)
			{
				foreach (Filth currentFilth in map.GetListerFilthInAreas()[priorityList[i]])
				{
					if (!currentFilth.DestroyedOrNull() && currentFilth.TicksSinceThickened >= WorkGiver_CleanFilthPrioritized.MinTicksSinceThickened)
					{
						prioritizedArea = priorityList[i];
						return;
					}
				}
			}
		}
	}
}