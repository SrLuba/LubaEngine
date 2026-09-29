# Luba Engine

Luba Engine is a 2D retro game engine written in C# on top of Raylib-cs,
designed for pixel-perfect, 16-bit style games. It is developed alongside
Mega Man X: Maverick Saga, but kept as a separate library so it can be reused
by other projects.

## Features
- Pixel-perfect rendering through low-resolution render textures per camera
- Palette swapping via shaders
- Sprite animation imported from Aseprite (layers, tags, per-frame data)
- Support for scanline type of backgrounds with hundred layers of parallax.
- Entity/component architecture with layered render queues
- Support for scenes
- Fixed 60 Hz logic timestep, independent from the render rate
- Bitmask-based input manager with game-defined actions
- Built-in ImGui dev tools (console, inspector, hierarchy), available in release builds through cmd arguments

## Roadmap
- Map Editor (Tilemap with support for multiple layers, solids, slopes, All divided in chunks with realtime culling)
- Node-Based Behaviour Editor (Visual Scripting)
- Simple Physics
- Fixed-point sub-pixel math for deterministic, frame-exact gameplay
- Platform-independent bytecode VM for game logic
- Long-term goal: runtimes for retro hardware (PS1 as baseline, Dreamcast)

## About AI usage
I'm completely against vibe coding.
All code in Luba Engine is written by hand. No AI tools are used to generate or autocomplete code.
AI is used only to look up library documentation and to discuss design ideas. 
All implementation is my own.
