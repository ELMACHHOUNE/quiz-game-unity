# CharacterGame

An interactive **educational quiz game** built with **Unity 6000.3.16f1**, featuring animated 2D characters, procedural UI, and multiple quiz categories.

**Authors:** Mohamed EL MACHHOUNE & Zakaria MAKLATI

---

## Features

- **2D Animated Characters** — PSD-imported characters with articulated idle, thinking, happy, angry, and celebrating animations (procedural C# coroutines, no Animator controllers)
- **3 Quiz Categories** — Code, Design, Mathematics — each with 10 hand-crafted questions
- **Quiz Mechanics** — Score tracking, combo multiplier, countdown timer per question, game-over summary
- **Dynamic UI** — All UI (menus, buttons, panels, pause screen) built programmatically at runtime
- **Procedural Background** — Parallax starfield with twinkling stars, drifting nebula blobs, mouse parallax, and click-burst particles
- **In-Game Pause Menu** — Resume, play again, or return to main menu during gameplay
- **Sound Toggle** — Music/sound on/off button in main menu

---

## Characters

Two PSB-imported characters with auto-generated GameObject hierarchies:

| Character | File | Position | Scale |
|---|---|---|---|
| Character 1 (left) | `Assets/Resources/CharacterPS-01.psb` | Viewport 15% left, 20% bottom | 0.18 (facing right) |
| Character 2 (right) | `Assets/Resources/character2.psb` | Viewport 85% left, 20% bottom | -0.18 (facing left, mirrored) |

Both characters use `CharacterEmotion.cs` for articulated animations and `CharacterAnchor.cs` for viewport-relative positioning.

---

## Quiz Categories

| Category | Icon | Quizzes |
|---|---|---|
| Code | 💻 | Git/GitHub, HTML Course |
| Design | 🎨 | Design Basics |
| Mathematics | 🔢 | Math Fundamentals |

---

## Project Structure

```
Assets/
├── Editor/
│   ├── BuildGame.cs           — Headless build pipeline
│   ├── DumpHierarchy.cs       — Debug hierarchy dumper
│   ├── PackageSetup.cs        — Automated package installation
│   └── PSDSetup.cs            — PSDImporter configuration
├── Resources/
│   ├── CharacterPS-01.psb     — Main character (left)
│   ├── character2.psb         — Second character (right)
│   ├── character.svg          — SVG fallback
│   ├── ELMACHHOUNE.png        — PNG fallback
│   └── sound-effect.mp3       — Background music
├── Scenes/
│   └── Main.unity             — Single scene (built empty, populated at runtime)
├── Scripts/
│   ├── MenuManager.cs         — Main menu, categories, navigation
│   ├── GameUI.cs              — In-game UI construction
│   ├── QuizGameManager.cs     — Quiz loop, scoring, timer
│   ├── QuestionGenerator.cs   — Question data & generation
│   ├── CharacterEmotion.cs    — Character animation states
│   ├── CharacterAnchor.cs     — Viewport anchoring
│   └── InteractiveBackground.cs — Parallax starfield background
└── Sprites/
    └── ELMACHHOUNE.png        — Standalone sprite asset
```

---

## How to Build

### From Unity Editor
1. Open the project in **Unity 6000.3.16f1**
2. Run **PSDSetup** (via script or manually ensure PSB importers are configured as Sprite/Multiple)
3. Run **BuildGame.PerformBuild** from the Editor or via command line

### Command Line
```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe" `
  -quit -batchmode `
  -projectPath "C:\path\to\CharacterGame" `
  -executeMethod BuildGame.PerformBuild `
  -logFile build_log.txt
```

The build outputs a **StandaloneWindows64** executable to `Build/CharacterGame.exe`.

---

## Key Packages

| Package | Version |
|---|---|
| `com.unity.2d.animation` | 13.0.5 |
| `com.unity.2d.psdimporter` | 12.0.2 |
| `com.unity.ugui` | 2.0.0 |
| `com.unity.ai.assistant` | 2.9.0-pre.2 |

Built with **Unity 6000.3.16f1**, targeting **Windows 64-bit** with the **Mono** scripting backend.

---

## Controls

| Action | Input |
|---|---|
| Answer question | Click one of 4 answer buttons |
| Pause | Click `☰ MENU` button (bottom-right) |
| Resume | Click `▶ RESUME` |
| Toggle sound | Click `♫ SOUND ON/OFF` (bottom-left in menu) |

---

## Development

- **Character layers** are organized in Photoshop PSD files with named groups (`head`, `left-hand`, `right-hand`, `body`, etc.)
- The PSDImporter generates a GameObject hierarchy matching the layer structure with `generateGOHierarchy: true`
- Body part finding relies on naming conventions (`left` + `hand`/`arm`, `head`/`face`, `right` + `hand`/`arm`)
- Sorting issues with hand sprites are resolved at runtime by elevating hand sprite sorting orders above the body maximum
