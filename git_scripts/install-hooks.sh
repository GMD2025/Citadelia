#!/bin/bash

REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null) || {
    echo "Not a git repository"
    exit 1
}

pre_commit() {
    SRC="$REPO_ROOT/git_scripts/pre-commit/script.sh"
    DEST="$REPO_ROOT/.git/hooks/pre-commit"

    if [[ ! -f "$SRC" ]]; then
        echo "Error: Source script not found!"
        exit 1
    fi

    if [[ ! -w "$REPO_ROOT/.git/hooks" ]]; then
        echo "Error: No write permission for .git/hooks"
        exit 1
    fi

    cp "$SRC" "$DEST"
    cp "$SRC_PS1" "$DEST"
    chmod +x "$DEST"
    echo "Pre-commit hook installed successfully."
}

pre_commit
