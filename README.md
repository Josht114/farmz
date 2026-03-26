# 🌾 FarmGame — 2D C# MonoGame Starter

A simple 2D farming game built with **MonoGame** (DesktopGL).

---

## Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0+ |
| [MonoGame templates](https://docs.monogame.net/articles/getting_started/1_setting_up_your_os.html) | 3.8.1 |

Install MonoGame templates once:
```
dotnet new --install MonoGame.Templates.CSharp
```

---

## Running the game

```bash
cd FarmGame
dotnet run
```

The first build will restore NuGet packages automatically.  
Placeholder coloured tiles appear until you add real art (see below).

---

## Controls

| Key | Action |
|-----|--------|
| WASD / Arrow keys | Move |
| `E` | Use equipped tool |
| `1` | Equip Hoe (plow soil) |
| `2` | Equip Seeds (plant) |
| `3` | Equip Scythe (harvest) |
| `Esc` | Quit |

---

## Project Structure

```
FarmGame/
├── Program.cs              Entry point
├── FarmGame.csproj         .NET / MonoGame project file
│
├── Core/
│   ├── Game1.cs            Main game loop, camera, render passes
│   └── TextureManager.cs   Central texture store + placeholder fallbacks
│
├── Entities/
│   ├── Tile.cs             TileType / CropStage enums + Tile struct
│   ├── WorldMap.cs         40×30 tile grid, procedural gen, plow/plant/harvest
│   └── Player.cs           Movement, tool use, inventory
│
├── Systems/
│   ├── InputHandler.cs     Keyboard → Player commands
│   └── TimeSystem.cs       In-game clock (minutes → hours → days → seasons)
│
├── UI/
│   └── HUD.cs              Toolbelt, clock, inventory overlay
│
└── Content/
    └── Content.mgcb        MonoGame content pipeline manifest
```

---

## Adding Art

1. Drop `.png` files into `Content/Textures/`
2. Register each in `Content/Content.mgcb`:
   ```
   /begin
   /importer:TextureImporter
   /processor:TextureProcessor
   /build:Textures/player.png
   /end
   ```
3. Load them in `TextureManager.Load()`:
   ```csharp
   PlayerTex = content.Load<Texture2D>("Textures/player");
   ```

---

## Adding a Font

1. Create a `.spritefont` XML file in `Content/Fonts/`
2. Register it in `Content.mgcb`
3. In `Game1.LoadContent()` the font is already loaded as `"DefaultFont"` — just name it that.

---

## Next Steps

- [ ] Add a shop / sell screen
- [ ] Watering-can mechanic (crops only grow when watered)
- [ ] Animated sprite sheets
- [ ] Saving / loading with `System.Text.Json`
- [ ] Multiple crop types
- [ ] NPC villagers
