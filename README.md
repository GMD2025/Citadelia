<a href="https://www.figma.com/board/PHm3roV1mkPZR1UrZZMZ5v/GMD?node-id=0-1&t=6pE2uI4GlM2q4ucD-1" target="_blank">
  <img src="https://upload.wikimedia.org/wikipedia/commons/3/33/Figma-logo.svg" alt="Figma" width="40" />
</a>

### NB! [How to run the game?](#how-toes)
### [Demo](https://youtu.be/QP5DW_KOAzk?si=nY0yo5D-Jfxk7CKP)

## Citadelia

Citadelia is a strategic tower defense game where players build and manage their own fortress to fend off waves of
enemies. The game combines resource management, tactical placement of buildings, and real-time combat mechanics to
create an engaging experience.

### Core Mechanics

- **Building Placement**: Drag and drop buildings onto a grid-based map to create your fortress.
- **Resource Management**: Collect and spend resources to construct and upgrade buildings.
- **Combat System**: Deploy units and defensive structures to protect your base from enemy attacks.
- **Symmetry-Based Gameplay**: Utilize mirrored tilemaps to create balanced and strategic layouts.

### Features

- **Dynamic Resource Production**: Buildings generate resources over time, enabling continuous growth.
- **AI-Controlled Units**: Enemy units dynamically navigate the map to attack your base.
- **Multiplayer Support**: Play with or against other players in a networked environment.
- **Customizable Grid Highlights**: Visualize building placement and restricted areas with interactive grid highlights.

## How-to'es

> Important to note:
> - the game is exclusively targeted for 2 players.
> - there is no functionality for split screen/local multiplayer inside the game
> - the multiplayer is run over the network

### How to play with a friend

In order to configure the IP address of the friend you want to play with, you have to first decide, who is going to be
the host, and who is going to join the running instance.

**The host:**  The host has it all easy - just run the `Citadelia.exe` and be the first one to start the game, i.e start
the host

**The second player:** Has to create `config.txt` file next to `Citadelia.exe` (has to be in the same folder).
The `txt` file should consist of two lines. The first one has to be your IPv4 address, the second will be just 7777

So the example of the config could be:

```
192.125.0.121
7777
```

> NB! **You and your friend have to be on the same network!**

Potentially, two instances of the game can be run on the same device, but with the joystick controls, you would not be
able to play them, so don't

### Controls

The game supports three input systems: Mouse/Touch, Keyboard, and Gamepad/Arcade.

#### 🖱️ Mouse / Touch

* **Highlighting grid tiles:** Move the pointer across the play area.
* **Placing buildings:** Drag a building from the building bar and drop it onto the grid.

#### ⌨️ Keyboard

* **Switch mode:** Press **F** to toggle between:

    * **Highlight Mode:** Move the highlighter across the grid.
    * **Choose Building Mode:** Navigate the building bar UI.
* **Navigation:**

    * In **Highlight Mode**, use arrow keys to move the highlighter on the grid.
    * In **Choose Building Mode**, use arrow keys to cycle through buildings in the building bar.
* **Confirm:**

    * In **Highlight Mode**, press **G** to place the selected building.
    * In **Choose Building Mode**, press **G** to select a building.

#### 🕹️ Gamepad / Arcade

* **Switch mode:** Press 🟡 (north button) to toggle between Highlight Mode and Choose Building Mode.
* **Navigation:**

    * Use the left stick or D-pad to move:

        * In **Highlight Mode**, move the highlighter on the grid.
        * In **Choose Building Mode**, browse the building bar.
* **Confirm:**

    * In **Highlight Mode**, press 🟢 (south button) to place the selected building.
    * In **Choose Building Mode**, press 🟢 to select a building.

## Collaboration

### Setup

After cloning this repository, run **`./git_scripts/install-hooks.sh`** in your terminal.  
This ensures that rules from [./Ignore/common-ignore.conf](./Ignore/common-ignore.conf) are correctly applied to
both `.gitignore` and `ignore.conf`.

> [!IMPORTANT]  
> **Windows users** must run the script using **Git Bash** or **WSL** to avoid compatibility issues.

> [!NOTE]  
> **Do not modify `.gitignore` or `ignore.conf` directly!**  
> To add specific rules, update [./Ignore/gitignore-template.conf](./Ignore/gitignore-template.conf)
> or [./Ignore/plasticignore-template.conf](./Ignore/plasticignore-template.conf) as needed.  
> For common rules, use [./Ignore/common-ignore.conf](./Ignore/common-ignore.conf).

### Keeping Git and Plastic SCM in Sync

> [!IMPORTANT]
> When pulling or pushing in Git, you **must** also apply changesets or pull in Unity VCS (Plastic SCM). It is **crucial
** to have the correct branches checked out in **both** repositories to avoid scenarios where changes are pushed to the
> wrong branch (e.g., pushing to a feature branch in Git but committing to `main` in Plastic SCM).

## Third-Party Assets

The following third-party assets and tools were used in this project:

- **Kenney Game Assets** – https://kenney.nl/assets  
  Used for building sprites and UI elements. Licensed
  under [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/).

- **Universal LPC Spritesheet Character Generator
  ** – https://liberatedpixelcup.github.io/Universal-LPC-Spritesheet-Character-Generator/  
  Used for player and unit character sprites. Based on [Liberated Pixel Cup assets](https://lpc.opengameart.org/).
  Licensed under [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/).

- **DOTween (Demigiant)** – http://dotween.demigiant.com/  
  Used for tweening animations and timed transitions. Licensed under the Demigiant license.

> Some visual or textual assets were generated with the help of AI tools (ChatGPT and Sora).

## Blogposts

Development logs and design notes are maintained in a separate repository:  
🔗 [GMD2025/Blogposts](https://github.com/GMD2025/Blogposts)

You can browse individual posts directly:

1. [Michael's Roll-a-ball](https://github.com/GMD2025/Blogposts/blob/main/1.%20Michael's%20Roll-a-ball.md) [Sevastian's Move-a-cube](https://github.com/GMD2025/Blogposts/blob/main/1.%20Sevastian's%20Move-a-cube.md)
2. [Game Design Document](https://github.com/GMD2025/Blogposts/blob/main/2.%20Game%20Design%20Document.md)
3. [Dev update!](https://github.com/GMD2025/Blogposts/blob/main/3.%20Dev%20update!.md)
4. [Working hard](https://github.com/GMD2025/Blogposts/blob/main/4.%20Working%20hard.md)
5. [In Development](https://github.com/GMD2025/Blogposts/blob/main/5.%20In%20Development.md)
6. [Final chapter](https://github.com/GMD2025/Blogposts/blob/main/6.%20Final%20chapter.md)
