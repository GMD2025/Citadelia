#!/bin/bash

# Script to convert Git ignore rules to Plastic SCM ignore rules (with inverse logic)
# Preserves explicit ignore rules from the original ignore.conf file

set -e # Exit on any error

# Get the Git repository root path
REPO_ROOT=$(git rev-parse --show-toplevel)

# Define file paths
ORIGINAL_IGNORE_CONF="$REPO_ROOT/git_scripts/pre-commit/ignore.conf"
PLASTIC_IGNORE="$REPO_ROOT/ignore.conf"
TEMP_FILE="$PLASTIC_IGNORE.tmp"

echo "Converting Git ignore rules to Plastic SCM ignore format..."

# Function to read ignore.conf and create a hash of explicitly ignored patterns
get_explicit_ignores() {
    local ignore_file="$1"
    local -n patterns_ref="$2"

    patterns_ref=()

    if [[ -f "$ignore_file" ]]; then
        while IFS= read -r line || [[ -n "$line" ]]; do
            # Skip comments and empty lines
            if [[ -n "$line" && ! "$line" =~ ^[[:space:]]*# && ! "$line" =~ ^[[:space:]]*$ ]]; then
                line="${line//[[:space:]]/}" # Trim spaces
                patterns_ref["$line"]=1
            fi
        done <"$ignore_file"
    fi
}

# Create the output file with headers
{
    echo "# Automatically generated from .gitignore with reversed rules"
    echo "# Generated on $(date)"
    echo "# This file inverts Git ignore rules for Unity Plastic SCM"
    echo ""
    echo "# Default rule - ignore everything by default"
    echo "*"
    echo ""
} >"$TEMP_FILE"

# Load explicit ignore patterns from original ignore.conf
declare -A explicit_ignores
get_explicit_ignores "$ORIGINAL_IGNORE_CONF" explicit_ignores

# Add original ignore.conf content if it exists
if [[ -f "$ORIGINAL_IGNORE_CONF" ]]; then
    {
        echo "# Original ignore.conf content"
        cat "$ORIGINAL_IGNORE_CONF"
        echo ""
    } >>"$TEMP_FILE"
fi

# Find all .gitignore files
echo "Finding .gitignore files..."
gitignore_files=$(git ls-files "*/.gitignore" ".gitignore")

# Process each .gitignore file
for gitignore_path in $gitignore_files; do
    echo "Processing $gitignore_path"

    full_path="$REPO_ROOT/$gitignore_path"
    if [[ ! -f "$full_path" ]]; then
        echo "Warning: Cannot find $full_path, skipping"
        continue
    fi

    echo "# Converted from $gitignore_path" >>"$TEMP_FILE"

    while IFS= read -r line || [[ -n "$line" ]]; do
        line="${line//[[:space:]]/}" # Trim spaces

        if [[ -z "$line" || "$line" =~ ^# ]]; then
            echo "$line" >>"$TEMP_FILE"
            continue
        fi

        if [[ "$line" =~ ^! ]]; then
            pattern="${line:1}"

            if [[ -z "${explicit_ignores[$pattern]}" ]]; then
                echo "$line" >>"$TEMP_FILE"
            else
                echo "# Skipped $line because it's explicitly ignored in ignore.conf" >>"$TEMP_FILE"
            fi
        else
            if [[ -n "${explicit_ignores[$line]}" ]]; then
                echo "# Skipped !$line because it's explicitly ignored in ignore.conf" >>"$TEMP_FILE"
            else
                echo "!$line" >>"$TEMP_FILE"
            fi
        fi
    done <"$full_path"

    echo "" >>"$TEMP_FILE"
done

# Ensure ignore.conf itself is never ignored
echo "# Ensure this file itself is never ignored" >>"$TEMP_FILE"
echo "!ignore.conf" >>"$TEMP_FILE"

# Replace the target file with the new generated file
mv "$TEMP_FILE" "$PLASTIC_IGNORE"

git add "$PLASTIC_IGNORE"

echo "✅ Plastic ignore.conf file created successfully at $PLASTIC_IGNORE"
