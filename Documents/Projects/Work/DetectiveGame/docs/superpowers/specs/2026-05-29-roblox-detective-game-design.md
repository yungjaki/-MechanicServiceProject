# Roblox Detective Game — Design Spec

**Date:** 2026-05-29
**Status:** Approved design, pre-implementation
**Working title:** (TBD) — a realistic detective/investigator game

## 1. Vision

A realistic investigator game in Roblox. The player is a detective who takes a case,
visits the crime scene, collects and analyzes evidence, interrogates suspects, reasons
on an evidence board, and must **prove and present** a case in court to convict the
culprit. Playable solo or co-op (1–4 players on the same side against an NPC culprit).

The defining principle: **you don't guess the answer — you build and defend an argument.**
A bare accusation gets thrown out. A conviction requires proving means, motive, and
opportunity with valid evidence.

## 2. Player structure

- **Solo and co-op**, same architecture. NPCs are suspects/witnesses; the culprit is an
  NPC defined by the case data (not a player).
- Co-op players are all on the same side, sharing case progress.

## 3. Architecture

### 3.1 Case engine, not hardcoded cases
The game is **one reusable engine** that loads a **Case File** (pure data) and runs the
investigation loop on it. Adding a new case = authoring new data, not new code. Cases are
hand-authored to start; the data-driven design leaves room for a procedural generator later
(explicitly out of scope for v1).

### 3.2 Server-authoritative truth
The solution (who's guilty, what each clue proves) lives **only on the server**. The client
receives only what the player has legitimately discovered. Benefits:
- **Anti-cheat:** players cannot datamine the answer.
- **Co-op for free:** discovered evidence is shared server state — works identically for 1 or
  4 players.

### 3.3 High-level components
- `CaseService` (server) — loads the active case, tracks discovered evidence per session,
  validates accusations, drives court logic.
- `CaseFiles/` (data ModuleScripts) — one module per case, pure data.
- Client controllers — scene interaction, evidence board UI, dialogue UI, court UI,
  tool/hotbar. Clients **request** actions; the server decides outcomes.
- `RemoteEvents`/`RemoteFunctions` — the only client/server bridge.

Flow: **player interacts → client fires remote → server checks case data → server replies
with what's revealed → client updates UI.**

## 4. The Case File (data model)

A case is a ModuleScript returning a table of this shape:

```
Case = {
  meta      = { title, victim, location, timeOfDeath },
  truth     = { culprit = "suspectId", motive = "...", weapon = "..." },  -- SERVER ONLY

  suspects  = {
     [id] = { name, model, alibi, isGuilty,
              statements = { ... },                        -- what they say
              lies       = { stmtId -> contradictedByClueId } }  -- catchable lies
  },

  clues     = {
     [id] = { name, foundAt = Vector3 / objectRef,
              revealTool = "uv" | "magnifier" | "flashlight" | "none",  -- what's needed to see it
              pillar     = "means" | "motive" | "opportunity" | nil,
              implicates = "suspectId"  OR  clears = "suspectId",
              labResult  = { requires = "dna" | "prints", reveals = "..." } }  -- optional
  },

  solution  = {
     culprit  = "suspectId",
     required = {                       -- what a conviction needs
        means       = { "clueId", ... },
        motive      = { "clueId", ... },
        opportunity = { "clueId", ... } }
  }
}
```

Key idea: **every clue knows what it proves and which suspect it points to or clears.**
The accusation system just checks whether the player linked the right clues to the right
pillars against the right person. Lies are the mirror of clues: a suspect statement is
flagged as a lie linked to the clue that disproves it, so interrogation and evidence
interlock.

The data model should also be able to express (designed for, light in v1): **red-herring
clues**, and clues/areas that unlock as progress is made.

## 5. Investigation loop

Six mechanics that chain together:

1. **Scene search** — interactable objects (`ClickDetector`/`ProximityPrompt`). Some clues
   are plain-sight; others require an equipped tool to reveal. Discovering a clue validates
   it server-side and adds it to the **Evidence Inventory** with a bag-and-tag feedback
   moment (sound + animation + UI).
2. **Tools** — a small hotbar (UV light → blood/fluids, magnifier → prints/fibers,
   flashlight → dark areas). Equipping changes what is revealable, making search active.
3. **Lab / forensics** — raw items (blood smear, phone) are taken to a lab station; a short
   analysis upgrades a clue into a *result* (e.g. "blood = victim's", "phone shows threat
   text"). Gates the strongest clues behind a step.
4. **Interrogation** — dialogue UI with suspect NPCs. Statements include data-flagged lies.
   Holding the contradicting clue unlocks a **"Present Evidence"** option on that statement;
   using it catches the lie, can crack an alibi, and reveals new info.
5. **Evidence board** — a pinboard UI; clues and suspects are cards. The player drags
   connections, assigning clues to **suspects** and to **pillars** (means/motive/opportunity).
   This is the player's own reasoning space — not auto-filled.
6. **Accusation → court** — see §6.

Throughline: **search finds raw clues → lab refines them → interrogation + board turn clues
into an argument → court tests that argument.** Each step has a clear input/output and is
independently buildable and testable.

## 6. Accusation & court (climax)

At the DA/courtroom:
1. **Name the culprit** from the suspect list.
2. **Assemble the case file** — slot evidence into each pillar (means / motive / opportunity).
3. **Present.** The server checks the file against the case `solution`:
   - **Right culprit + all three pillars proven with valid clues → conviction** (win).
   - **Right culprit but a pillar weak/missing → the DA pushes back** with a data-defined
     rejection line ("You haven't established *how*."); player returns to investigate. Not an
     outright loss.
   - **Wrong culprit, or proof that points elsewhere → case thrown out** (optional consequence
     such as reputation hit / culprit escapes this playthrough).

The DA "poking holes" is data-driven: each pillar requirement has a rejection line if unmet.
No AI needed for v1 — scripted dialogue keyed to what's missing. Prevents savescumming a bare
name; the player must build the argument.

## 7. Look, feel & asset strategy

Production value comes from a **curation + polish pipeline**, not from procedurally
hand-sculpting models.

- **Models / maps:** Roblox Creator Store + Toolbox (furniture, props, rooms, character
  models); PolyHaven for realistic textures/HDRIs. User curates/approves; assistant imports
  and arranges.
- **Animations:** Roblox animation library + marketplace packs; custom ones via Studio's
  animation editor (user records, assistant hooks into code).
- **Sounds:** Roblox audio library + free SFX (ambient room tone, camera shutter, UV hum,
  page flips, tense stings). Assistant wires triggers; user approves picks.
- **GUI:** directly built to a premium standard by the assistant — a cohesive "case file /
  detective HUD" theme (typography, color, layout, transitions, hover states).
- **Polish layer as a design principle:** every interaction gets a juice pass (sound +
  animation + UI feedback) as a real per-feature task, not an afterthought.

**Division of labor:** assistant builds systems + GUI + wiring and edits directly in Roblox
Studio (via the Studio integration when connected; otherwise provides copy-paste-ready files);
user curates/approves visual & audio assets, with the assistant suggesting specific ones.

**Honest expectation:** v1 is functional and cohesively styled. "AAA-realistic" is an
ongoing asset-curation effort the user drives over time; the engine does not change for it.

## 8. MVP scope — Case #1: "The Penthouse"

A single-location murder, tight enough to finish and rich enough to exercise every system.

- **One location:** a furnished apartment (imported assets). Victim found dead.
- **3 suspects:** e.g. business partner, ex, neighbor. One guilty (data-defined); two with
  breakable alibis.
- **~6–8 clues:** 2 plain-sight, 2 tool-gated (UV blood, magnifier prints), 2 lab-processed
  (DNA match, phone threat text), plus 1–2 red herrings.
- **Each mechanic appears at least once, minimally:** search, tools (UV + magnifier), lab
  (one analysis), interrogation (one catchable lie per suspect), board, court (3 pillars).
- **Fully solvable** with a real proof chain, e.g.: *means* = weapon + prints; *opportunity*
  = broken alibi + UV trail; *motive* = phone threat + lab DNA.

### Out of scope for v1 (YAGNI)
Multiple locations, save/persistence between sessions, leaderboards, a second case, the
case generator, dynamic AI dialogue, character customization. The architecture leaves slots
for these; they are not built in v1.

### Co-op
Server-authoritative from day one. Build and verify solo first, then validate 2-player. Not
a rewrite — just validation.

## 9. Testing strategy

- **Logic without art:** `CaseService` accusation/validation logic is plain Luau tables,
  testable in isolation (simulate "player has clues X,Y,Z, accuses suspect B" → assert
  verdict) before any models exist.
- **In-Studio playtest per mechanic:** search reveals the right clue; tool-gating works; lab
  upgrades a clue; presenting a clue catches a lie; board links persist; court verdict is
  correct for right / weak / wrong cases.
- **Polish pass per mechanic** after it works (sound + animation + UI feedback).
- The assistant states explicitly what was verified vs. what requires the user to playtest in
  Studio (the assistant cannot "feel" the game).

## 10. Open items / dependencies

- **Roblox Studio integration:** not confirmed connected in the design session. Verify the
  link before the build phase; fall back to copy-paste-ready files if unavailable.
- **Game title:** TBD.
- **Project location:** `DetectiveGame/` under the working directory.
