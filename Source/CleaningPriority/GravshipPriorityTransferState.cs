using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace CleaningPriority;

internal class GravshipPriorityTransferState : WorldComponent
{
    private List<string> capturedPriorityLabels = [];

    public GravshipPriorityTransferState(World world) : base(world)
    {
    }

    public bool HasCapturedPriorities => capturedPriorityLabels.Any();

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref capturedPriorityLabels, "capturedPriorityLabels", LookMode.Value);
        capturedPriorityLabels ??= [];
    }

    public void CapturePriorities(IEnumerable<string> labels)
    {
        capturedPriorityLabels = labels.Where(label => !string.IsNullOrEmpty(label)).Distinct().ToList();
    }

    public List<string> ConsumePriorities()
    {
        var labels = capturedPriorityLabels;
        capturedPriorityLabels = [];
        return labels;
    }
}