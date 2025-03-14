#!/bin/bash

# Path to your Git repository
REPO_ROOT=$(git rev-parse --show-toplevel)

# Path to the ignore.conf file
IGNORE_CONF="$REPO_ROOT/git_scripts/pre-commit/ignore.conf"

# New plastic ignore file location (to be written to $REPO_ROOT/ignore.conf)
PLASTIC_IGNORE="$REPO_ROOT/ignore.conf"

# Function to convert .gitignore to plastic ignore
convert_gitignore_to_plasticignore() {
    local gitignore_path="$1"
    local plasticignore_path="$2"
    local temp_file="${plasticignore_path}.tmp"

    # Create a temporary file for our processed content
    >"$temp_file"

    # Headers at the very beginning
    echo "# Automatically generated from .gitignore with reversed rules" >"$temp_file"
    echo "# Generated on $(date)" >>"$temp_file"
    echo "# This file inverts Git ignore rules for Unity Plastic SCM" >>"$temp_file"
    echo "" >>"$temp_file"

    # Processed .gitignore rules in the middle
    while IFS= read -r line || [[ -n "$line" ]]; do
        # Handle section dividers and comments as-is
        if [[ "$line" =~ ^# ]]; then
            echo "$line" >>"$temp_file"
            continue
        fi

        # Handle empty lines by preserving them exactly
        if [[ -z "$line" ]]; then
            echo "" >>"$temp_file"
            continue
        fi

        # Check if the line contains only whitespace
        if [[ "$line" =~ ^[[:space:]]*$ ]]; then
            echo "$line" >>"$temp_file"
            continue
        fi

        # Process negation rules (lines starting with !)
        if [[ "$line" =~ ^\! ]]; then
            # For Git lines that start with ! (don't ignore), Plastic should ignore them
            # So remove the ! and add the path
            modified_line="${line:1}"
            echo "$modified_line" >>"$temp_file"
        else
            # For Git lines that are ignored, Plastic should NOT ignore them
            # So add ! at the beginning
            echo "!$line" >>"$temp_file"
        fi
    done <"$gitignore_path"

    echo "" >>"$temp_file"

    # Ignore.conf content (if it exists)
    if [[ -f "$IGNORE_CONF" ]]; then
        echo "########################### Original ignore.conf content ###########################" >>"$temp_file"
        cat "$IGNORE_CONF" >>"$temp_file"
        echo "" >>"$temp_file"
    fi

    # The catch-all "*" rule at the very end
    echo "# Default rule - ignore everything not explicitly included" >>"$temp_file"
    echo "*" >>"$temp_file"

    # Replace the target file with our temporary file
    mv "$temp_file" "$plasticignore_path"
}

# For Windows compatibility, use Git's own methods to find files
echo "Finding .gitignore files..."
gitignore_files=$(git ls-files "*/.gitignore" ".gitignore")

# Process each found .gitignore file
for gitignore_file in $gitignore_files; do
    echo "Processing $gitignore_file -> $PLASTIC_IGNORE"
    convert_gitignore_to_plasticignore "$REPO_ROOT/$gitignore_file" "$PLASTIC_IGNORE"
done

# Stage the new ignore.conf file
git add "$PLASTIC_IGNORE"

echo "Plastic ignore.conf file created successfully!"
