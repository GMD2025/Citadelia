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
