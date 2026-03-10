# GrokDungeon Design

## Feasibility and Vision
GrokDungeon aims to serve as an extensible wrapper for LLM-driven Game Masters (LLM DMs) with a persistent world state. By combining an Entity Component System (ECS) architecture, an embedded NoSQL database (RavenDB), and a standardized AI API client layer (via Microsoft.Extensions.AI and OpenAI compatibility), the project is highly feasible.

The current foundation supports 5e-style combat and exploration. The transition from specific rulesets (e.g., 5e to Pathfinder or FFG Star Wars) can be achieved by generalizing the ECS components and abstracting the game logic modules. The prompt injected into the LLM dictates narrative style and rule enforcement, while the ECS logic validates actions and manages the hard state (HP, inventory, location).

## Architecture

1. **ECS (DefaultEcs):** Represents the game state. Entities are players, NPCs, items, and rooms. Components are stats, health, location, etc.
2. **Persistence (RavenDB):** Stores the long-term state. The ECS world is synced to and from RavenDB documents for saving and loading.
3. **AI Interface (Microsoft.Extensions.AI):** Abstracted chat interface. We use `OpenAIClient` and its compatibility with other providers (Grok, Gemini) to offer flexible LLM choices.
4. **Tag System (TagExecutor):** The LLM outputs strict XML actions (e.g., `<update>`, `<action>`) that are parsed and executed by the game engine, bridging the gap between narrative intent and hard state modification.

## Ruleset Agnosticism

To achieve switching between rulesets via configuration:

- **Component Generalization:** Instead of hardcoding 5e stats (Strength, Dexterity, etc.), use a flexible property bag or generic `StatComponent` that loads ruleset definitions from JSON.
- **Rule Engines:** Introduce `IRuleEngine` interface. Concrete implementations (`Dnd5eRuleEngine`, `PathfinderRuleEngine`, `FfgStarWarsRuleEngine`) handle combat resolution, skill checks, and dice parsing.
- **Dynamic Prompts:** The system prompt should be dynamically constructed based on the active ruleset config.

## Spec for "Client": Musical Threats

In FFG Star Wars (or general cinematic gameplay), a "Client" can emit background music or themes based on the current threat level, narrative beats, or room tags.

**Musical Threats (Dynamic Audio Spec)**

**Concept:**
The LLM dictates the mood or the threat level implicitly via narrative, or explicitly via a new XML tag (`<mood type="combat" intensity="high" />`). The client maps this state to background tracks.

**Flow:**
1. **LLM Output:** The prompt is updated to instruct the LLM to output a `<mood>` tag during scene transitions or escalations.
2. **Tag Executor:** Parses `<mood>` and updates a global `MoodComponent` on a singleton "World" entity.
3. **Audio Service:** An `IAudioService` monitors the `MoodComponent`.
    - If `type=combat` and `intensity=high`, it crossfades to the "Boss Battle" track.
    - If `type=exploration` and `intensity=low`, it plays "Ambient Dungeon".
4. **Client Implementation:** The console client (or a future web/desktop client) receives these mood events and triggers actual audio playback (e.g., via NAudio in C# or browser Audio API).
