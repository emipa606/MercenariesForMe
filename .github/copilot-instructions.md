# GitHub Copilot Instructions for Mercenaries For Me (Continued)

## Mod Overview and Purpose

**Mercenaries For Me (Continued)** is an update of the original mod by aRandomKiwi, built upon the 1.5 version by Ionfrigate12345. It enables players to hire and rent mercenaries for various roles, from soldiers to diplomats, within the RimWorld game. The mod is seamlessly integrated with RimWorld's game mechanics, offering flexible gameplay enhancement and support for other mods. It features a dynamic system that adapts to the player's research level, enabling either a modern or medieval mode of play.

## Key Features and Systems

- **Mercenary Hiring System**: Players can hire mercenaries, who are fully controllable like colonists, through different in-game interactions—via caravans, traders, or through certain mods like "nopower comms console."
- **Contract System**: Mercenary contracts are based on the in-game quadrum system, which offers flexibility and scalability as a player's needs change.
- **Research-Based Mode**: Automatically switches between modern and medieval mercenaries based on the player's research progression.
- **Cross-Mod Compatibility**: Supports weapons and gear from other mods, including compatibility with Combat Extended.
- **Incidents and Interactions**: Various in-game events related to hired mercenaries, such as counter-offers, mercenary deaths, and faction relations, create dynamic challenges.
- **Customization**: Extensive customization options for tailoring the mercenary experience to fit individual gameplay styles.

## Coding Patterns and Conventions

The coding structure adheres to standard C# conventions:
- Classes are named in PascalCase, such as `Alert_PlannedDeliveryStuffAndGuarantee`.
- Methods follow the camelCase convention, e.g., `removeSOP()`.
- Use of partial classes and methods is minimized for clarity.
- Utilities and extensions are encapsulated in static classes like `Utils`.

## XML Integration

XML files are integrated to define attributes of mercenary types, equipment, and faction settings, enabling easy customization. The XML files are typically utilized for:
- Defining mercenary roles and characteristics.
- Specifying gear compatibility and requirements.
- Setting up faction-specific dialogues and interactions.

## Harmony Patching

The mod extensively uses Harmony for patching base game methods, with files following the internal class and method structure:
- **Patch Files**: Located under files like `Building_CommsConsole_Patch.cs`, each with classes like `Internal class Building_CommsConsole_GetFloatMenuOption_Patch` that modify specific game functionalities.
- **Methods**: Harmony patches are applied using methods like `Postfix` and `Prefix` for altering existing game methods without direct modification.

## Suggestions for Copilot

To maximize the assistant's effectiveness, consider the following when generating code with Copilot:
- Indicate the specific game aspect you are modifying or enhancing for more relevant suggestions.
- Make sure Harmony prefixes or postfixes are logical and context-specific to avoid game-breaking changes.
- For XML files, specify the section you are updating for concise and accurate XML generation.
- Use descriptive comments in your code and XML to guide Copilot in understanding your needs and providing better autocomplete suggestions.

With these instructions, contributors can efficiently extend or update the Mercenaries For Me mod while maintaining consistency and quality. This mod offers a rich set of features that heavily integrate with RimWorld's base game mechanics to enhance the gameplay experience significantly.
