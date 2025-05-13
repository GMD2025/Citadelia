<a href="https://www.figma.com/board/PHm3roV1mkPZR1UrZZMZ5v/GMD?node-id=0-1&t=6pE2uI4GlM2q4ucD-1" target="_blank">
  <img src="https://upload.wikimedia.org/wikipedia/commons/3/33/Figma-logo.svg" alt="Figma" width="40" />
</a>

## Citadelia

Citadelia is a strategic tower defense game where players build and manage their own fortress to fend off waves of enemies. The game combines resource management, tactical placement of buildings, and real-time combat mechanics to create an engaging experience.

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

## Collaboration

### Setup

After cloning this repository, run **`./git_scripts/install-hooks.sh`** in your terminal.  
This ensures that rules from [./Ignore/common-ignore.conf](./Ignore/common-ignore.conf) are correctly applied to both `.gitignore` and `ignore.conf`.

> [!IMPORTANT]  
> **Windows users** must run the script using **Git Bash** or **WSL** to avoid compatibility issues.

> [!NOTE]  
> **Do not modify `.gitignore` or `ignore.conf` directly!**  
> To add specific rules, update [./Ignore/gitignore-template.conf](./Ignore/gitignore-template.conf) or [./Ignore/plasticignore-template.conf](./Ignore/plasticignore-template.conf) as needed.  
> For common rules, use [./Ignore/common-ignore.conf](./Ignore/common-ignore.conf).

### Keeping Git and Plastic SCM in Sync

> [!IMPORTANT]
> When pulling or pushing in Git, you **must** also apply changesets or pull in Unity VCS (Plastic SCM). It is **crucial** to have the correct branches checked out in **both** repositories to avoid scenarios where changes are pushed to the wrong branch (e.g., pushing to a feature branch in Git but committing to `main` in Plastic SCM).
