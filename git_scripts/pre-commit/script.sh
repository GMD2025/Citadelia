#!/bin/bash

REPO_ROOT=$(git rev-parse --show-toplevel)

"$REPO_ROOT/git_scripts/sync_ignore_files.sh"

git rm --cached -r . >/dev/null 2>&1
git add --all >/dev/null 2>&1

echo "✅ pre-commit hook executed successfully"
