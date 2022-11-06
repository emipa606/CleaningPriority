using System.Reflection;
using HarmonyLib;
using Verse;

namespace CleaningPriority;

[StaticConstructorOnStartup]
internal class CleaningPriorityInitialization
{
    static CleaningPriorityInitialization()
    {
        var harmony = new Harmony("com.github.chippedchap.cleaningpriority");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}