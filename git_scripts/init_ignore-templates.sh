#!/bin/bash

# Find the root of the Git repository
REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null)

# Check if the repo root was found
if [ -z "$REPO_ROOT" ]; then
    echo "⚠️  Error: Not inside a Git repository."
    exit 1
fi

# Define paths
IGNORE_DIR="$REPO_ROOT/Ignore"
GITIGNORE_SRC="$REPO_ROOT/.gitignore"
PLASTICIGNORE_SRC="$REPO_ROOT/ignore.conf"
GITIGNORE_TEMPLATE="$IGNORE_DIR/gitignore-template"
PLASTICIGNORE_TEMPLATE="$IGNORE_DIR/plasticignore-template"
COMMON_IGNORE="$IGNORE_DIR/common-ignore.conf"

# Ensure Ignore directory exists
mkdir -p "$IGNORE_DIR"

# Copy .gitignore to gitignore-template
if [ -f "$GITIGNORE_SRC" ]; then
    cp "$GITIGNORE_SRC" "$GITIGNORE_TEMPLATE"
    echo "✅ Updated $GITIGNORE_TEMPLATE"
else
    echo "⚠️  Warning: $GITIGNORE_SRC not found, skipping."
fi

# Copy ignore.conf to plasticignore-template
if [ -f "$PLASTICIGNORE_SRC" ]; then
    cp "$PLASTICIGNORE_SRC" "$PLASTICIGNORE_TEMPLATE"
    echo "✅ Updated $PLASTICIGNORE_TEMPLATE"
else
    echo "⚠️  Warning: $PLASTICIGNORE_SRC not found, skipping."
fi

# Create common-ignore.conf if it does not exist
if [ ! -f "$COMMON_IGNORE" ]; then
    touch "$COMMON_IGNORE"
    echo "# Common ignore rules (initial empty file)" >"$COMMON_IGNORE"
    echo "✅ Created $COMMON_IGNORE"
fi
