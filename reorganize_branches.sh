#!/bin/bash

# Branch Reorganization Script for DialoguePlus
# This script reorganizes the repository branches as follows:
# 1. Archives main to old_unity
# 2. Renames console_dev to old_console
# 3. Replaces main with dev contents

set -e  # Exit on any error

echo "=========================================="
echo "DialoguePlus Branch Reorganization Script"
echo "=========================================="
echo ""

# Check if we're in a git repository
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "Error: Not in a git repository"
    exit 1
fi

echo "Fetching all branches from remote..."
git fetch --all

echo ""
echo "Step 1: Creating old_unity branch from current main..."
git checkout main
git checkout -b old_unity
git push origin old_unity
echo "✓ old_unity branch created and pushed"

echo ""
echo "Step 2: Renaming console_dev to old_console..."
git checkout console_dev
git branch -m old_console
git push origin old_console
git push origin --delete console_dev
echo "✓ console_dev renamed to old_console"

echo ""
echo "Step 3: Updating main to match dev..."
git checkout main
git reset --hard dev
git push origin main --force
echo "✓ main branch updated with dev contents"

echo ""
echo "=========================================="
echo "Branch reorganization complete!"
echo "=========================================="
echo ""
echo "Current remote branches:"
git branch -r | grep -E "origin/(main|old_unity|old_console|dev)" || git branch -r

echo ""
echo "Summary of changes:"
echo "  - main: Now contains the dev branch content (C# console application)"
echo "  - old_unity: Contains the original main branch (Unity project)"
echo "  - old_console: Contains the original console_dev branch"
echo ""
echo "Note: You may want to update the default branch in GitHub settings if needed."
