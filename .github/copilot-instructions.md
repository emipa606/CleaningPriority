# GitHub Copilot Instructions for Cleaning Priority (Continued) Mod

## Mod Overview and Purpose

The Cleaning Priority (Continued) mod is an update to the ChippedChaps mod, designed to prioritize cleaning areas in a RimWorld game. It allows players to set different cleaning priorities for various areas, ensuring that certain areas, such as hospitals, are cleaned before others. The mod also includes features like Russian translations and correct reservation of filth to prevent multiple pawns from cleaning the same filth simultaneously.

## Key Features and Systems

### Priority Cleaning
- **Area Prioritization**: Players can assign priorities to different areas. Colonists will clean higher priority areas before lower priority ones.
- **Arbitrary Priorities**: Players can set any area as a priority, even removing the default home area and replacing it with a custom area.

### Filth Reservation
- **Correct Filth Reservation**: Ensures that pawns do not clean the same filth simultaneously, improving efficiency.

### Compatibility
- **Soft Incompatibility with Dubs Bad Hygiene**: Pawns may clean outside of designated zones. Disabling "prefer indoor cleaning" in DBH options resolves this issue.

## Coding Patterns and Conventions

### C# Coding Patterns
- **Modular Design**: The code is organized into multiple classes, each responsible for specific tasks such as managing cleaning areas, tracking filth, and handling area changes.
- **Harmony Patching**: The mod uses Harmony to patch existing game methods without detouring them, ensuring compatibility with other mods.

### XML Integration
- **XML Patches**: The mod uses XML files to define Harmony patches, allowing for easy modification of game behavior without altering the original codebase.
- **Translation Files**: XML files are used for translations, making it easy to add support for new languages.

## Harmony Patching

The Cleaning Priority (Continued) mod uses Harmony to patch the following game methods:
- **WorkGiverPatch.xml**: This XML file contains patches to the `WorkGiver` class to integrate the new cleaning logic.

### Example Harmony Patch
xml
<Patch>
  <Operation Class="HarmonyLib.CodeInstructionTranspiler">
    <Target Method="WorkGiver_GatherItems:JobOnCell"/>
    <Transpiler Class="CleaningPriority.HarmonyPatches.WorkGiver_GatherItems_Transpiler"/>
  </Operation>
</Patch>


### C# Example of a Harmony Transpiler
csharp
using HarmonyLib;
using RimWorld;

namespace CleaningPriority.HarmonyPatches
{
    [HarmonyPatch(typeof(WorkGiver_GatherItems), nameof(WorkGiver_GatherItems.JobOnCell))]
    public static class WorkGiver_GatherItems_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            // Modify the instructions to integrate cleaning priority logic
            // Example: codes.Insert(0, new CodeInstruction(OpCodes.Ldarg_0));
            return codes;
        }
    }
}


## Suggestions for Copilot

- **Mod Integration**: When suggesting code, consider how it integrates with the existing modular design and Harmony patching system.
- **XML Patches**: When suggesting XML code, ensure that it adheres to the Harmony patching conventions and is placed in the correct file.
- **Translation Support**: When adding new translations, ensure that the XML files are updated to include the new language.
- **Testing**: Always include comments or instructions for testing the new code to ensure it works as intended without causing bugs or incompatibilities.

By following these guidelines, you can contribute effectively to the Cleaning Priority (Continued) mod, ensuring that it remains robust and compatible with other RimWorld mods.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

