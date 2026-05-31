# Backrooms Game — Foundation Design (Sub-Project 1)

**Date:** 2026-05-31
**Status:** Approved (design); pending implementation plan
**Engine:** Unity (URP)

---

## Context & The Bigger Picture

We are building a realistic 3D Backrooms horror game in Unity. The full vision —
procedurally generated, ever-different maps that stream in as the player explores
deeper, multiple Backrooms "levels," and realistic scary/bloody entities, drawing on
both Kane Pixels' found-footage original and the Backrooms Wiki community canon — is
too large for a single spec. It decomposes into four sub-projects, each with its own
spec → plan → build cycle:

1. **Foundation** (this spec) — first-person core loop + one beautiful, walkable
   classic Level 0 area that proves the look and movement.
2. **Procedural generation** — infinite, different-every-run layouts that stream in,
   plus noclip transitions between levels.
3. **Entities** — creatures (Smiler, Hound, Skin-Stealer, etc.), their AI, and the
   scary/gory presentation.
4. **Asset pipeline** — Blender-built/sourced realistic models, textures, props.

Build order is deliberate: nail the Foundation first. You cannot tune a variation
system before one room looks right to vary from.

## Key Decisions (and why)

- **Engine: Unity.** User's choice; strong realism + huge free asset ecosystem.
  No Unity MCP exists, so the user drives the editor with step-by-step guidance while
  Claude writes all code and builds assets in Blender.
- **Pipeline: URP, not HDRP.** Target hardware is a **GTX 1050 Ti** (4GB VRAM, no RT
  cores), which sits below HDRP's comfortable range. The Backrooms look is ~90%
  lighting, fog, texture quality, and post-processing — all of which URP delivers at
  high fidelity while running smoothly on the 1050 Ti. "Realistic" comes from PBR
  textures + baked lighting, not from the pipeline; URP will not look pixelated.
- **First look: Classic yellow Level 0.** Fastest route to a recognizable result.
  Kane/gritty and other looks come later as variations.
- **Variety via grade profiles.** User wants different looks in different places
  (some areas "A/classic", some "B/Kane"). Handled as a core feature: a room reads its
  look from a `BackroomsGradeProfile` asset; new looks are new assets, no code changes.
- **Build approach: ProBuilder blockout → Blender PBR kit (Option 1).** Block out fast
  in-editor to lock scale and get walking immediately, then replace with a modular
  Blender PBR kit. The modular pieces double as the building blocks the procedural
  generator will need later.

## Scope of This Slice

**In scope:** A single atmospheric classic-yellow Level 0 area (one room opening into
an L-shaped hallway) that the player walks around in first-person, looking genuinely
real and running smoothly on the 1050 Ti.

**Explicitly out of scope (later sub-projects):** procedural generation, multiple
levels, entities, sanity/stamina/inventory systems, save/load.

### Definition of Done
- Launch → spawn first-person inside the room.
- Walk, look, sprint, with footstep sounds and the iconic fluorescent hum.
- Room reads as real: correct scale, PBR walls/carpet/ceiling, baked lighting, fog,
  subtle post-processing (film grain, slight chromatic aberration, vignette, bloom).
- Stable framerate, target 60fps, on the GTX 1050 Ti.
- One short connected space (room + L-hallway), not infinite.

## Architecture / Build Order

1. **Project setup** — new Unity URP project; install ProBuilder, Cinemachine, Input
   System; clean folder structure; Unity `.gitignore` + version control.
2. **Blockout** — box out room + L-hallway in ProBuilder. Lock scale (ceiling ~3m,
   hallway ~2m wide). Walk it immediately with a temporary capsule.
3. **Player controller** — first-person walk/sprint/look, head-bob, footsteps,
   mouse-look via Cinemachine, built on the new Input System.
4. **Blender modular kit** — wall panel, doorway, floor tile, ceiling tile +
   fluorescent fixture, with PBR textures (Blender + PolyHaven). Import and replace
   the blockout.
5. **Realism layer** — PBR materials (albedo/normal/roughness; wet-ish carpet
   roughness), baked lighting via Progressive Lightmapper (emissive baked fluorescents,
   optional one real-time flickering light), URP distance fog, post-processing Volume
   (sickly yellow-green grade, film grain, chromatic aberration, vignette, subtle
   bloom), and audio (3D-positioned fluorescent hum, room tone, footsteps).
6. **Grade-profile stub** — `BackroomsGradeProfile` ScriptableObject holding wall/
   floor/ceiling material refs, fog color+density, light color+intensity, % dead tubes,
   post-processing intensity. Ship one profile (`Classic`) now; future looks are new
   profile assets consumed per-zone by the generator later.

## Components / Isolation

- **PlayerController** — input → movement/look. Depends on Input System + Cinemachine.
  Testable by walking; no knowledge of room contents.
- **GradeProfile (ScriptableObject)** — pure data describing a look. No dependencies.
- **RoomDresser / look applier** — reads a `GradeProfile` and applies materials, fog,
  lights, post settings to the scene. Depends on GradeProfile; swappable seam for
  future variation.
- **Modular kit (Blender assets)** — geometry + materials. Consumed by the scene now,
  by the generator later.

## Verification

Claude cannot see the Unity editor, so the user is the eyes — reporting what they see
and sharing screenshots at each milestone, tuning from there.

- **After blockout:** walk it; confirm scale *feels* right (not doll-house, not giant).
- **After controller:** smooth movement, no jitter; add a quick FPS counter and check
  framerate.
- **After kit + realism layer:** compare side-by-side with reference — does it read as
  real? Framerate still ~60fps?

## Risks / Notes

- **Performance on the 1050 Ti** is the standing constraint. Baked lighting, fog-based
  far culling, and cheap post effects are the mitigations. Re-check FPS at each stage.
- **Scale errors** silently kill the liminal feel — validated explicitly at blockout.
- **No Unity MCP** — all editor steps must be explicit, click-by-click guidance.
