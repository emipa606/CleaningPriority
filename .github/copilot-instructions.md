# GitHub Copilot Instructions for the Cleaning Priority (Continued) Mod

## Mod Overview and Purpose
Cleaning Priority (Continued) is a mod for RimWorld that allows players to assign custom cleaning priorities to different areas, providing greater control over their colony's cleaning efforts. The mod updates the original by ChippedChap and adds new features such as correct reservation of filth and compatibility adjustments for Russian translations.

## Key Features and Systems

### Priority Cleaning
- **Objective:** Prioritize colonist cleaning tasks by assigning specific areas to different priority levels.
- **Usage:** Through an in-game menu accessible via a broom icon, players can set priority levels for different areas, ensuring essential places like hospitals are cleaned before less critical zones.

### Arbitrary Priorities
- **Customization:** Players can create custom priority lists, removing default home areas from priority if desired. 
- **Flexibility:** Regardless of the list's configuration, colonists will clean according to this custom priority system.

### Delays and Filth Tracking
- **Delay Handling:** Adds a slight delay in recognizing new dirt, akin to vanilla mechanics, necessitating filth presence for 600 ticks before action.

### Compatibility
- **Integrations:** Maintains vanilla filth tracking and ensures no core methods are detoured, promoting seamless operation with other mods.
- **Incompatibilities:** Some interaction issues with mods like Dubs Bad Hygiene, but resolved by tweaking specific settings.

## Coding Patterns and Conventions
- **Class Accessibility:** Utilize `internal` keyword for encapsulation unless broader access is necessary.
- **Naming Conventions:** Descriptive C# naming for clarity, e.g., `Area_Set`, `JobDriver_CleanPrioritizedFilth`.
- **Separation of Concerns:** Follow single-responsibility principles within components, e.g., `ListerFilthInHomeArea_Notify_FilthSpawned` focuses on filth events.

## XML Integration
- **Purpose:** Allow for the definition and modification of game definitions, such as structured data that represents area priorities.
- **Usage:** XML files specify mod metadata, translations, and configurations that are read at runtime.

## Harmony Patching
- **Avoidance:** Avoid direct method detours to preserve compatibility. Use Harmony patches selectively where necessary to inject custom logic without altering original game code.

## Suggestions for Copilot
1. **Prioritize Crafting Helper Methods:** Implement quick utility methods for managing 'area' collections and priority order processing.
2. **Refactorism Proposals:** Copilot should suggest breaking down complex tasks in classes like `CleaningManager_MapComponent` into smaller helper methods. 
3. **Unit Tests:** Generate ideas for unit test scaffolds that evaluate core functionalities such as filth prioritization logic.
4. **Feasible Feature Suggestions:** Leverage Copilot for brainstorming possible new features or optimizations, like enhanced UI for priority settings.

## Additional Notes
- **Stability:** Soft-incompatibilities and expected bugs in early versions should be anticipated; however, they rarely corrupt saves.
- **Feedback and Bugs:** Users encountering issues should isolate mod conflicts and report logs through recommended channels.

Keep exploring modding possibilities for broader cleaning task management within RimWorld for improved colony micro-management!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
