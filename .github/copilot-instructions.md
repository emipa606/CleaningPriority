# GitHub Copilot Instructions for Cleaning Priority (Continued)

## Mod Overview and Purpose

**Mod Name:** Cleaning Priority (Continued)  
**Author:** Mlie  
**Package ID:** Mlie.CleaningPriority

The "Cleaning Priority (Continued)" mod is an enhancement and maintenance update of the original mod by ChippedChap. The mod aims to give players more control over the cleaning priorities of their colonists in RimWorld, allowing specific areas to be prioritized over others. This is especially useful in maintaining high-traffic or critical areas like hospitals or kitchens clean at all times.

## Key Features and Systems

- **Priority Cleaning:** Allows players to assign cleaning priorities to different areas. Colonists will clean higher-priority areas first.
- **Arbitrary Priorities:** Custom areas can be prioritized for cleaning over default home areas, ensuring adaptable cleaning behavior.
- **Default Behavior Compatibility:** If the priority system is not adjusted, colonists will clean using the default home area cleaning behavior.
- **Ease of Use:** The mod provides a user interface via the broom icon to manage cleaning priorities easily.
- **Compatibility:** Filth tracking does not detour vanilla methods, maintaining compatibility with other mods that use vanilla cleaning mechanisms.

## Coding Patterns and Conventions

- **C# Patterns:** Object-oriented design principles are used extensively. Classes like `CleaningManager_MapComponent` manage the priority logic and state.
- **Method Naming:** Methods follow a CamelCase naming convention and are descriptive of their functionality, e.g., `EnsureHasAtLeastOneArea()`.
- **Members and Types:** A combination of data structures is used for maintaining lists of areas and priorities, utilizing Lists and Dictionaries.

## XML Integration

- **Patch Structure:** XML files like `WorkGiverPatch.xml` are used for Harmony patching and integrating with the game’s job driver and work types.
- **Localization:** XML files are also utilized for localizing content and integrating a Russian translation provided by Vladimir Saenko.

## Harmony Patching

The mod uses Harmony, a library for patching methods in compiled assemblies, allowing non-invasive patches to RimWorld's existing functionality. For this mod:

- **Patch Locations:** XML and starting methods (`CleaningPriorityInitialization.cs`) are patched to introduce new cleaning logic.
- **Compatibility:** Ensures no original game methods are fully detoured, keeping it compatible with older tracking systems.

## Suggestions for Copilot

1. **Code Completion:** Suggest auto-completion for repetitive patterns in method definitions and member variables.

2. **Refactoring Help:** Provide suggestions for refactoring long methods into smaller, more manageable functions to improve readability.

3. **Enhancement Ideas:** Propose adding features, such as setting a timeout for how long an area remains prioritized or feedback mechanisms for when priority lists become too cluttered.

4. **Error Handling:** Recommend adding more structured error-handling patterns when interfacing with the game's API, to manage potential conflicts with other mods effectively.

5. **Translation Extension:** Assist in expanding additional translations or creating templates for easier integration of multiple languages beyond the current Russian translation.

## Additional Notes

- **Compatibility Tips:** If issues arise, strip the mod list down to core dependencies and reintroduce other mods incrementally.
- **Bug Reporting:** Encourage use of the Discord channel for support and the in-game log uploader for debugging assistance.
- **User Guidance:** Highlight the usage of RimSort for organizing mods, ensuring they are loaded in a compatible order.

By following these guidelines and leveraging GitHub Copilot effectively, developers can ensure the mod remains robust, maintainable, and enjoyable for users.

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

