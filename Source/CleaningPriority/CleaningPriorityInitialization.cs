using System.Reflection;
using HarmonyLib;
using Verse;

namespace CleaningPriority;

[StaticConstructorOnStartup]
internal class CleaningPriorityInitialization
{
    static CleaningPriorityInitialization()
    {
        new Harmony("com.github.chippedchap.cleaningpriority").PatchAll(Assembly.GetExecutingAssembly());
    }
}