# GrokDungeon

GrokDungeon is an open-source, extensible wrapper for LLM-driven Game Masters (LLM DMs) featuring a persistent world state. It combines an Entity Component System (ECS) architecture, an embedded NoSQL database (RavenDB), and a standardized AI API client layer to deliver an immersive and adaptable tabletop role-playing experience directly in your console.

## Features

- **LLM Game Master:** Uses large language models (like Grok, OpenAI ChatGPT, and Google Gemini) to dictate narrative and control non-player characters and encounters.
- **Entity Component System (ECS):** Represents the game state using `DefaultEcs`. Entities (players, NPCs, items, rooms) are managed efficiently.
- **Persistent State:** Uses an embedded RavenDB to seamlessly save and load your game world.
- **Spectre.Console Interface:** Provides a rich, stylized terminal interface out of the box.
- **AI Provider Agnostic:** Configurable AI providers leveraging the `Microsoft.Extensions.AI` abstractions.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/GrokDungeon.git
   cd GrokDungeon
   ```

2. **Configure your AI Provider:**
   The game relies on an AI provider. In `GrokDungeon/appsettings.json`, select your preferred provider. Out of the box, we support `Grok`, `OpenAI`, and `Gemini`.

   Example configuration:
   ```json
   "GrokDungeon": {
     "AiProviders": {
       "SelectedProvider": "Grok",
       "Providers": {
         "Grok": {
           "Endpoint": "https://api.x.ai/v1",
           "Model": "grok-beta",
           "ApiKeyEnvVar": "XAI_API_KEY"
         },
         "OpenAI": {
           "Endpoint": "https://api.openai.com/v1",
           "Model": "gpt-4o-mini",
           "ApiKeyEnvVar": "OPENAI_API_KEY"
         },
         "Gemini": {
           "Endpoint": "https://generativelanguage.googleapis.com/v1beta/openai/",
           "Model": "gemini-2.5-flash",
           "ApiKeyEnvVar": "GEMINI_API_KEY"
         }
       }
     }
   }
   ```

3. **Set your API Key:**
   Depending on the provider you selected, you need to set the corresponding environment variable (e.g., `XAI_API_KEY`, `OPENAI_API_KEY`, or `GEMINI_API_KEY`).

   ```bash
   export XAI_API_KEY="your-api-key-here"
   ```

4. **Run the Game:**
   ```bash
   dotnet run --project GrokDungeon
   ```

## Playground Console App

Use `GrokDungeon.Playground` to quickly preview Spectre.Console rendering widgets and iterate on UI elements without running the full game loop.

```bash
dotnet run --project GrokDungeon.Playground
```

## Contributing

We welcome contributions! Please feel free to submit pull requests or open issues for bugs and feature requests. See the [DESIGN.md](DESIGN.md) document to understand the underlying architecture and future directions, such as ruleset agnosticism and dynamic audio clients.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
