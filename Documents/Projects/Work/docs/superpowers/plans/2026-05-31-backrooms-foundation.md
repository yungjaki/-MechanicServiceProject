# Backrooms Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a single, atmospheric, walkable classic-yellow Backrooms Level 0 area in first-person that looks realistic and runs at ~60fps on a GTX 1050 Ti.

**Architecture:** Unity URP project. Block out the room + L-hallway with ProBuilder to lock scale, add a script-based first-person controller, replace blockout geometry with a modular PBR kit built in Blender, then layer in baked lighting, fog, post-processing, and audio. A `GradeProfile` ScriptableObject captures the look so future variants are data-only.

**Tech Stack:** Unity 6 LTS (6000.0.x), URP, ProBuilder, C# (legacy Input Manager), Blender (via MCP) + PolyHaven textures.

---

## Who does what

- **YOU (the user)** drive the Unity Editor — every click-path is spelled out. There is no Unity MCP.
- **CLAUDE** writes all C# code, and builds the 3D kit + textures in Blender via the Blender MCP, then hands you files to import.
- **Verification** is by playing and observing (Claude can't see your editor). Each task ends with "what you should see" and a screenshot check where it matters. Code/asset files are committed to the project's own git repo.

## Adjustments from spec (intentional, reversible)
- Legacy Input Manager (not new Input System) — zero setup for a beginner.
- Script-based first-person look (not Cinemachine) — one fewer system to learn.
- Unity project + its own git repo at `C:\Users\dimit\Documents\Projects\Home\Backrooms`. Design docs stay in the Work repo.

## File Structure (created across this plan)

Inside the Unity project (`.../Home/Backrooms/`):
- `Assets/_Backrooms/Scripts/FirstPersonController.cs` — movement + look + headbob + footsteps
- `Assets/_Backrooms/Scripts/FpsCounter.cs` — on-screen framerate readout
- `Assets/_Backrooms/Scripts/GradeProfile.cs` — ScriptableObject: a look definition
- `Assets/_Backrooms/Scripts/GradeProfileApplier.cs` — applies a GradeProfile to the scene
- `Assets/_Backrooms/Art/Kit/` — imported Blender modular kit (.fbx + textures)
- `Assets/_Backrooms/Materials/` — PBR materials
- `Assets/_Backrooms/Audio/` — hum, room tone, footstep clips
- `Assets/_Backrooms/Profiles/Classic.asset` — the first GradeProfile instance
- `Assets/Scenes/Level0.unity` — the playable scene
- `.gitignore` — Unity gitignore

---

### Task 0: Install Unity Editor and create the URP project

**Files:** none yet (editor setup).

- [ ] **Step 1: Open Unity Hub → Installs tab**

In Unity Hub, click **Installs** (left sidebar). If you see a version like **6000.0.x LTS** already installed, skip to Step 3.

- [ ] **Step 2: Install Unity 6 LTS**

Click **Install Editor** (top right) → choose the latest **6000.0.x (LTS)** → in **Add modules**, leave defaults (no extra platforms needed) → **Install**. This downloads ~8GB; let it finish.

- [ ] **Step 3: Create the project**

Hub → **Projects** → **New project** → select the **6000.0.x LTS** editor → pick the **Universal 3D** template (this is URP; it may be named "Universal 3D (URP)"). 
- Project name: `Backrooms`
- Location: `C:\Users\dimit\Documents\Projects\Home`
Click **Create project**. Unity opens (first open is slow — it's compiling).

- [ ] **Step 4: Verify**

You should see the Unity Editor open with a default URP sample scene. Confirm the bottom-left shows no compile errors (no red error count). Tell Claude: "project open, no errors" (or paste any errors).

---

### Task 1: Project hygiene — git, gitignore, folders, ProBuilder

**Files:**
- Create: `C:\Users\dimit\Documents\Projects\Home\Backrooms\.gitignore`

- [ ] **Step 1: Create the folder structure**

In Unity's **Project** window (bottom panel), right-click the `Assets` folder → **Create → Folder**. Create a folder named `_Backrooms`. Inside `_Backrooms`, create these subfolders the same way: `Scripts`, `Art`, `Materials`, `Audio`, `Profiles`. (The leading underscore keeps it sorted to the top.)

- [ ] **Step 2: Add the Unity .gitignore (Claude provides)**

Claude creates `.gitignore` at the project root with this content:

```gitignore
# Unity generated
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
[Mm]emoryCaptures/
*.csproj
*.sln
*.user
.vs/
.idea/
# OS
.DS_Store
Thumbs.db
```

- [ ] **Step 3: Initialize git for the project**

Run (Claude executes):

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" init
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add .gitignore
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "chore: init Unity project with gitignore"
```

- [ ] **Step 4: Install ProBuilder**

In Unity: **Window → Package Manager**. Top-left dropdown → **Unity Registry**. Search `ProBuilder` → select it → **Install**. Wait for it to finish (progress bar bottom-right).

- [ ] **Step 5: Verify**

Open **Tools → ProBuilder → ProBuilder Window**. A small toolbar window should appear. Tell Claude: "ProBuilder installed."

- [ ] **Step 6: Commit the asset folders**

Unity writes `.meta` files for your new folders. Run (Claude executes):

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "chore: add project folder structure"
```

---

### Task 2: Blockout the room + L-hallway (lock the scale)

**Files:**
- Create: `Assets/Scenes/Level0.unity`

- [ ] **Step 1: Make and save the scene**

**File → New Scene** → pick **Basic (URP)** → **Create**. Then **File → Save As** → navigate to `Assets/Scenes` → name it `Level0` → **Save**.

- [ ] **Step 2: Create the floor with correct scale**

Open the ProBuilder window (**Tools → ProBuilder → ProBuilder Window**). Click **New Shape** → choose **Cube**. In the **Create Shape** settings (Scene view, bottom-right or the tool overlay), set Size to **X=8, Y=0.2, Z=8** (an 8×8 metre room, 20cm-thick floor slab). Click in the scene to place it, then set its Transform **Position = (0, 0, 0)** in the **Inspector**.

- [ ] **Step 3: Build four walls**

For each wall, **New Shape → Cube**, then set size + position in the Inspector. Ceiling height = 3m, walls 0.2m thick:
- Wall North: Size (8, 3, 0.2), Position (0, 1.5, 4)
- Wall South: Size (8, 3, 0.2), Position (0, 1.5, -4)
- Wall East: Size (0.2, 3, 8), Position (4, 1.5, 0)
- Wall West: Size (0.2, 3, 8), Position (-4, 1.5, 0)

- [ ] **Step 4: Add a ceiling**

**New Shape → Cube**, Size (8, 0.2, 8), Position (0, 3, 0).

- [ ] **Step 5: Cut a doorway and add the L-hallway**

In the North wall, we want an opening. Simplest beginner approach: instead of cutting, delete Wall North and replace with two short wall segments leaving a 1.2m gap in the middle:
- Wall North-Left: Size (3.4, 3, 0.2), Position (-2.3, 1.5, 4)
- Wall North-Right: Size (3.4, 3, 0.2), Position (2.3, 1.5, 4)

Then build an L-shaped hallway extending north then turning east, 2m wide, 3m tall. Create as cubes (floor/ceiling/walls) using the same method:
- Hall floor A: Size (2, 0.2, 4), Position (0, 0, 6)
- Hall ceiling A: Size (2, 0.2, 4), Position (0, 3, 6)
- Hall wall A-West: Size (0.2, 3, 4), Position (-1, 1.5, 6)
- Hall wall A-East: Size (0.2, 3, 4), Position (1, 1.5, 6)
(The east side opens into the turn.) Then the east leg:
- Hall floor B: Size (4, 0.2, 2), Position (3, 0, 7)
- Hall ceiling B: Size (4, 0.2, 2), Position (3, 3, 7)
- Hall wall B-North: Size (4, 3, 0.2), Position (3, 1.5, 8)
- Hall wall B-South: Size (4, 3, 0.2), Position (3, 1.5, 6)

- [ ] **Step 6: Group the blockout**

In the **Hierarchy**, create an empty: right-click → **Create Empty**, rename it `Blockout`, set its Position to (0,0,0), and drag all the cube objects onto it to parent them. Save the scene (**Ctrl+S**).

- [ ] **Step 7: Verify scale (screenshot checkpoint)**

We can't judge scale until you walk it — that happens in Task 3. For now confirm in the Scene view that the room looks like a room (not a shoebox or a stadium). Send Claude a screenshot of the Scene view. We adjust dimensions here if it looks off.

- [ ] **Step 8: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: blockout Level0 room and L-hallway"
```

---

### Task 3: First-person controller (walk, sprint, look, headbob, footsteps)

**Files:**
- Create: `Assets/_Backrooms/Scripts/FirstPersonController.cs`

- [ ] **Step 1: Create the player object**

In the Hierarchy: right-click → **Create Empty**, rename `Player`, Position (0, 1, 0). With `Player` selected, **Add Component → Character Controller**. In the Character Controller, set **Height = 1.8**, **Radius = 0.3**, **Center = (0, 0, 0)**.

- [ ] **Step 2: Parent the camera to the player**

Drag the existing **Main Camera** in the Hierarchy onto `Player` to parent it. Select Main Camera, set its **Position = (0, 0.7, 0)** (eye height above player center) and Rotation (0,0,0).

- [ ] **Step 3: Claude writes the controller script**

Claude creates `Assets/_Backrooms/Scripts/FirstPersonController.cs`:

```csharp
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.2f;
    public float sprintSpeed = 4.2f;
    public float gravity = -18f;

    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float pitchClamp = 85f;

    [Header("Head Bob")]
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.04f;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    private CharacterController _controller;
    private float _pitch;
    private float _verticalVelocity;
    private Vector3 _camStartLocalPos;
    private float _bobTimer;
    private float _stepTimer;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (cameraTransform != null) _camStartLocalPos = cameraTransform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        if (cameraTransform == null) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up, mouseX);
        _pitch = Mathf.Clamp(_pitch - mouseY, -pitchClamp, pitchClamp);
        cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = sprinting ? sprintSpeed : walkSpeed;

        Vector3 move = (transform.right * x + transform.forward * z).normalized * speed;

        if (_controller.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move + Vector3.up * _verticalVelocity;
        _controller.Move(velocity * Time.deltaTime);

        bool moving = new Vector2(x, z).sqrMagnitude > 0.01f && _controller.isGrounded;
        HeadBob(moving, speed);
        Footsteps(moving, sprinting);
    }

    void HeadBob(bool moving, float speed)
    {
        if (cameraTransform == null) return;
        if (moving)
        {
            _bobTimer += Time.deltaTime * bobFrequency * (speed / walkSpeed);
            float offsetY = Mathf.Sin(_bobTimer) * bobAmplitude;
            cameraTransform.localPosition = _camStartLocalPos + new Vector3(0f, offsetY, 0f);
        }
        else
        {
            _bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, _camStartLocalPos, Time.deltaTime * 8f);
        }
    }

    void Footsteps(bool moving, bool sprinting)
    {
        if (footstepSource == null || footstepClips == null || footstepClips.Length == 0) return;
        if (!moving) { _stepTimer = 0f; return; }
        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            _stepTimer = sprinting ? stepInterval * 0.6f : stepInterval;
            var clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSource.PlayOneShot(clip);
        }
    }
}
```

- [ ] **Step 4: Attach and wire the script**

Select `Player` → **Add Component** → type `First Person Controller` → add it. In the component, drag the **Main Camera** (from Hierarchy) into the **Camera Transform** field. Leave footstep fields empty for now (wired in Task 9).

- [ ] **Step 5: Verify by playing**

Press **Play** (top center). Click in the Game view to capture the mouse. You should be able to: walk with **WASD**, look with the mouse (pitch clamped so you can't flip over), **sprint with Left Shift**, and feel a subtle head bob while walking. You should not fall through the floor. Press **Play** again to stop. 

**This is the scale checkpoint:** does walking through the room and hallway *feel* right — like a real building, not a dollhouse or a giant warehouse? Tell Claude how it feels; we tweak `walkSpeed` and/or room dimensions if not.

- [ ] **Step 6: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/_Backrooms/Scripts/FirstPersonController.cs Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: first-person controller with headbob"
```

---

### Task 4: On-screen FPS counter (so we can watch performance)

**Files:**
- Create: `Assets/_Backrooms/Scripts/FpsCounter.cs`

- [ ] **Step 1: Claude writes the script**

```csharp
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    private float _deltaTime;
    private GUIStyle _style;

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (_style == null)
        {
            _style = new GUIStyle();
            _style.fontSize = 24;
            _style.normal.textColor = Color.white;
        }
        float fps = 1f / Mathf.Max(_deltaTime, 0.0001f);
        GUI.Label(new Rect(10, 10, 200, 30), $"{fps:0.} FPS", _style);
    }
}
```

- [ ] **Step 2: Attach it**

In the Hierarchy create an empty named `Debug`, **Add Component → Fps Counter**.

- [ ] **Step 3: Verify**

Press **Play** — a white "xx FPS" readout appears top-left. Note the number (should be high right now, the scene is empty). Stop play.

- [ ] **Step 4: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/_Backrooms/Scripts/FpsCounter.cs Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: on-screen fps counter"
```

---

### Task 5: Build the modular PBR kit in Blender (Claude, via MCP)

**Files:**
- Create (Claude, in Blender then exported): `Assets/_Backrooms/Art/Kit/wall_panel.fbx`, `floor_tile.fbx`, `ceiling_tile.fbx`, `doorway.fbx`
- Create: PBR texture files under `Assets/_Backrooms/Art/Kit/Textures/`

- [ ] **Step 1: Claude models the kit in Blender**

Claude uses the Blender MCP to model four modular pieces on a 1m grid, real-world scale (metres), with clean UVs:
- `wall_panel` — 2m wide × 3m tall × 0.1m thick panel
- `floor_tile` — 2m × 2m × 0.1m slab
- `ceiling_tile` — 2m × 2m × 0.1m with a recessed rectangular fluorescent fixture
- `doorway` — wall panel with a 1.2m × 2.1m opening

- [ ] **Step 2: Claude sources/sets PBR textures**

Claude pulls suitable PBR texture sets via the Blender PolyHaven integration (damp yellow wallpaper for walls, worn low-pile carpet for floor, stained ceiling tile), and assigns them with correct scale.

- [ ] **Step 3: Claude exports to the Unity project**

Claude exports each piece as `.fbx` (Y-up, metres, apply transforms) into `Assets/_Backrooms/Art/Kit/` and copies texture maps into `.../Kit/Textures/`.

- [ ] **Step 4: You import / let Unity pick them up**

Switch to Unity — it auto-imports the new files (you'll see them appear in the Project window under `_Backrooms/Art/Kit`). If an import dialog appears, accept defaults. Tell Claude: "kit imported" or paste any import warnings.

- [ ] **Step 5: Verify**

Drag `wall_panel` from the Project window into the Scene view to confirm it appears at sensible size (about 3m tall next to your Player). Then **delete** that test drag (we place them properly in Task 6). 

- [ ] **Step 6: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/_Backrooms/Art
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: import modular Backrooms kit from Blender"
```

---

### Task 6: Replace the blockout with the kit + build materials

**Files:**
- Create: `Assets/_Backrooms/Materials/Wallpaper.mat`, `Carpet.mat`, `CeilingTile.mat`

- [ ] **Step 1: Create the materials**

In `Assets/_Backrooms/Materials`, right-click → **Create → Material**, name `Wallpaper`. In its Inspector (URP Lit shader), assign **Base Map** = the wallpaper albedo texture, **Normal Map** = wallpaper normal, set **Smoothness** low (~0.15). Repeat for `Carpet` (albedo/normal, Smoothness ~0.25 — slightly damp sheen) and `CeilingTile` (Smoothness ~0.1).

- [ ] **Step 2: Place kit pieces over the blockout**

Hide the `Blockout` group (select it, uncheck the checkbox at top of Inspector). Drag kit prefabs/models into the scene to recreate the room walls/floor/ceiling and hallway at the same coordinates from Task 2, snapping to the 1m grid (hold **Ctrl** while moving to grid-snap; verify grid in **Edit → Grid and Snap**). Group them under a new empty named `Level0_Geo`.

- [ ] **Step 3: Apply materials**

Drag `Wallpaper` onto wall pieces, `Carpet` onto floors, `CeilingTile` onto ceilings (drag the material from Project onto the object in Scene or Hierarchy).

- [ ] **Step 4: Mark geometry as static (needed for baking later)**

Select `Level0_Geo` → in the Inspector top-right, click the **Static** dropdown arrow → enable **Contribute GI** and **Occluder/Occludee Static** (or just tick **Static** for all). Apply to children when prompted.

- [ ] **Step 5: Verify**

Press **Play** and walk around. It should now look like a real (if unlit) Backrooms room with proper wallpaper/carpet/ceiling textures. Check the **FPS** readout — note the number. Send Claude a screenshot. We fix texture tiling/scale here if anything looks stretched.

- [ ] **Step 6: Delete the blockout and commit**

Once the kit version looks right, delete the `Blockout` group from the Hierarchy. Save scene.

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/_Backrooms/Materials Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: replace blockout with PBR kit and materials"
```

---

### Task 7: Lighting — fluorescent fixtures + baked GI

**Files:** scene-only changes (`Assets/Scenes/Level0.unity`) + generated lightmap assets.

- [ ] **Step 1: Remove default lighting that doesn't fit**

The Backrooms has no sun. Select the **Directional Light** in the Hierarchy and **disable** it (uncheck), or delete it.

- [ ] **Step 2: Add fluorescent lights**

For each ceiling fixture, create a light: right-click Hierarchy → **Light → Area Light** (best for baked soft light) positioned just below each ceiling fixture, rotated to point **down**. Set **Color** to a slightly cold white (e.g. RGB ~ 235, 240, 230), **Intensity** moderate. Set the light **Mode = Baked** (in the light's Inspector). Add ~4–6 across the room and hallway.

- [ ] **Step 3: Make the fixture meshes glow (emissive)**

Select the `CeilingTile` material's fixture sub-material (or make a `FixtureGlow` material): enable **Emission**, set emission color to the same cold white, and tick **Global Illumination = Baked** so the glow contributes to the bake.

- [ ] **Step 4: Configure the bake**

**Window → Rendering → Lighting**. In the **Scene** tab: ensure **Baked Global Illumination** is ON, set **Lightmapper = Progressive GPU** (faster on your card; fall back to CPU if it errors), **Lightmap Resolution** modest (e.g. 20–30 texels/unit to keep bake fast). Click **Generate Lighting** (bottom). Wait for the bake (watch the progress bottom-right).

- [ ] **Step 5: Verify**

Press **Play** — the room should now have soft, realistic bounced light and glowing ceiling fixtures, with gentle shadows. Check FPS (baked light is cheap, should stay high). Screenshot to Claude.

- [ ] **Step 6: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: baked fluorescent lighting"
```

---

### Task 8: Fog + post-processing (the cinematic grade)

**Files:** scene-only changes + a URP Volume Profile asset (auto-created).

- [ ] **Step 1: Enable fog**

**Window → Rendering → Lighting → Environment** tab → enable **Fog**. Mode **Exponential**, a low **Density** (~0.03), color a desaturated dark yellow-grey. This hides the hallway's end and helps performance.

- [ ] **Step 2: Add a post-processing Volume**

Right-click Hierarchy → **Volume → Global Volume**. In its Inspector, next to **Profile**, click **New** to create a profile asset (save under `_Backrooms`). 

- [ ] **Step 3: Add overrides**

On the Volume, **Add Override** for each:
- **Color Adjustments** — Post Exposure slightly down, Saturation −10, tint toward sickly yellow-green.
- **Film Grain** — Type Medium, Intensity ~0.3.
- **Chromatic Aberration** — Intensity ~0.1.
- **Vignette** — Intensity ~0.3.
- **Bloom** — Threshold ~1.0, Intensity ~0.3 (makes the tubes glow).

- [ ] **Step 4: Make sure the camera renders post**

Select **Main Camera** → in **Rendering**, ensure **Post Processing** is checked.

- [ ] **Step 5: Verify**

Press **Play** — it should now read distinctly as "Backrooms found-footage": yellow-green sickly grade, grain, soft glow on tubes, fog down the hall. Check FPS (post is cheap; confirm still smooth). Screenshot to Claude — this is the big "does it look real?" checkpoint.

- [ ] **Step 6: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/Scenes Assets/_Backrooms
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: fog and post-processing grade"
```

---

### Task 9: Audio — fluorescent hum, room tone, footsteps

**Files:**
- Add: `Assets/_Backrooms/Audio/hum.wav`, `room_tone.wav`, `footstep_1.wav`..`footstep_4.wav`

- [ ] **Step 1: Get the clips**

Claude advises sourcing royalty-free clips (e.g. freesound.org): a looping fluorescent buzz/hum, a low room-tone drone, and 3–4 carpet footstep sounds. Place the files in `Assets/_Backrooms/Audio`. (Tell Claude if you want help picking specific ones.)

- [ ] **Step 2: 3D hum on a fixture**

Select one ceiling fixture object → **Add Component → Audio Source** → set **AudioClip = hum**, **Loop = on**, **Spatial Blend = 1 (3D)**, low **Volume**. Duplicate onto a couple of other fixtures so the buzz follows you.

- [ ] **Step 3: 2D room tone**

On the `Player` (or an empty `Ambience`) add an **Audio Source** → **AudioClip = room_tone**, **Loop on**, **Spatial Blend = 0 (2D)**, very low volume.

- [ ] **Step 4: Wire footsteps**

On the `Player`, add a second **Audio Source** (this is the footstep source; **Play On Awake = off**, **Spatial Blend = 0**). Select `Player` → in **First Person Controller**, drag this Audio Source into **Footstep Source**, and set **Footstep Clips** size = 4, dragging in `footstep_1..4`.

- [ ] **Step 5: Verify**

Press **Play** — you should hear the buzz get louder near lights, a constant low room tone, and footsteps timed to walking (faster when sprinting). Stop.

- [ ] **Step 6: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/_Backrooms/Audio Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: ambient audio and footsteps"
```

---

### Task 10: Grade-profile architecture (future-proofing variety)

**Files:**
- Create: `Assets/_Backrooms/Scripts/GradeProfile.cs`
- Create: `Assets/_Backrooms/Scripts/GradeProfileApplier.cs`
- Create: `Assets/_Backrooms/Profiles/Classic.asset`

- [ ] **Step 1: Claude writes the ScriptableObject**

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "GradeProfile", menuName = "Backrooms/Grade Profile")]
public class GradeProfile : ScriptableObject
{
    [Header("Materials")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material ceilingMaterial;

    [Header("Fog")]
    public Color fogColor = new Color(0.45f, 0.42f, 0.30f);
    public float fogDensity = 0.03f;

    [Header("Lighting")]
    public Color lightColor = new Color(0.92f, 0.94f, 0.90f);
    [Range(0f, 8f)] public float lightIntensity = 1.0f;
    [Range(0f, 1f)] public float deadTubeFraction = 0f;

    [Header("Post")]
    [Range(0f, 1f)] public float postIntensity = 1f;
}
```

- [ ] **Step 2: Claude writes the applier**

```csharp
using UnityEngine;

public class GradeProfileApplier : MonoBehaviour
{
    public GradeProfile profile;

    [Tooltip("Renderers whose look this profile drives, by role.")]
    public Renderer[] wallRenderers;
    public Renderer[] floorRenderers;
    public Renderer[] ceilingRenderers;
    public Light[] fixtureLights;

    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        if (profile == null) return;

        AssignMaterial(wallRenderers, profile.wallMaterial);
        AssignMaterial(floorRenderers, profile.floorMaterial);
        AssignMaterial(ceilingRenderers, profile.ceilingMaterial);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = profile.fogColor;
        RenderSettings.fogDensity = profile.fogDensity;

        if (fixtureLights != null)
        {
            int deadCount = Mathf.RoundToInt(fixtureLights.Length * profile.deadTubeFraction);
            for (int i = 0; i < fixtureLights.Length; i++)
            {
                if (fixtureLights[i] == null) continue;
                bool dead = i < deadCount;
                fixtureLights[i].enabled = !dead;
                fixtureLights[i].color = profile.lightColor;
                fixtureLights[i].intensity = dead ? 0f : profile.lightIntensity;
            }
        }
    }

    void AssignMaterial(Renderer[] renderers, Material mat)
    {
        if (renderers == null || mat == null) return;
        foreach (var r in renderers)
            if (r != null) r.sharedMaterial = mat;
    }
}
```

- [ ] **Step 3: Create the Classic profile asset**

In `Assets/_Backrooms/Profiles`, right-click → **Create → Backrooms → Grade Profile**, name it `Classic`. In its Inspector assign `Wallpaper`, `Carpet`, `CeilingTile` materials and confirm the default fog/light values match what you tuned in Tasks 7–8.

- [ ] **Step 4: Wire the applier in the scene**

On `Level0_Geo` (or a new empty `LookController`) → **Add Component → Grade Profile Applier**. Set **Profile = Classic**. Populate the renderer arrays by dragging the wall/floor/ceiling objects in, and the fixture **Light** objects into `fixtureLights`.

- [ ] **Step 5: Verify**

Press **Play** — the scene should look identical to before (the profile reproduces the current look). Then, as a test, in the `Classic` asset set **Dead Tube Fraction = 0.25** and Play again — about a quarter of the lights should be off, proving the profile drives the look. Set it back to 0. Screenshot to Claude.

- [ ] **Step 6: Commit**

```bash
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" add Assets/_Backrooms Assets/Scenes
git -C "C:\Users\dimit\Documents\Projects\Home\Backrooms" commit -m "feat: grade profile system with Classic profile"
```

---

## Definition of Done (foundation)

- Launch → first-person spawn inside the room. ✔ Task 3
- Walk/look/sprint + footsteps + fluorescent hum. ✔ Tasks 3, 9
- Reads as real: PBR materials, baked light, fog, post. ✔ Tasks 6, 7, 8
- Stable ~60fps on the 1050 Ti (checked at Tasks 4, 6, 7, 8). ✔
- One connected space (room + L-hallway), not infinite. ✔ Tasks 2, 6
- Look driven by a `GradeProfile` so variety is data-only later. ✔ Task 10

## Out of scope (next sub-projects)
Procedural generation, multiple levels, entities, sanity/stamina/inventory, save/load.
