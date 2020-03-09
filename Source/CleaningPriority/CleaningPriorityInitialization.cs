using HarmonyLib;
using System.Reflection;
using Verse;

namespace CleaningPriority
{
	[StaticConstructorOnStartup]
	class CleaningPriorityInitialization
	{
		static CleaningPriorityInitialization()
		{
			var harmony = new Harmony("com.github.chippedchap.cleaningpriority");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}