#!/bin/bash

# Branch Reorganization Script for DialoguePlus
# This script reorganizes the repository branches as follows:
# 1. Archives main to old_unity
# 2. Renames console_dev to old_console
# 3. Replaces main with dev contents

set -e  # Exit on any error

# Parse arguments
DRY_RUN=false
if [ "$1" = "--dry-run" ] || [ "$1" = "-n" ]; then
    DRY_RUN=true
    echo "Running in DRY-RUN mode (no changes will be made)"
fi

echo "=========================================="
echo "DialoguePlus Branch Reorganization Script"
echo "=========================================="
echo ""

# Check if we're in a git repository
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "Error: Not in a git repository"
    exit 1
fi

if [ "$DRY_RUN" = false ]; then
    # Warning message
    echo "⚠️  WARNING: This script will:"
    echo "   1. Create a new branch 'old_unity' from current 'main'"
    echo "   2. Rename 'console_dev' to 'old_console'"
    echo "   3. OVERWRITE 'main' with contents from 'dev' (using --force)"
    echo ""
    echo "This operation will change the default branch content!"
    echo ""
    read -p "Do you want to continue? (yes/no): " confirm

    if [ "$confirm" != "yes" ]; then
        echo "Operation cancelled."
        exit 0
    fi
fi

echo ""
echo "Fetching all branches from remote..."
git fetch --all

echo ""
echo "Step 1: Creating old_unity branch from current main..."
git checkout main

# Check if old_unity already exists locally
if git show-ref --verify --quiet refs/heads/old_unity; then
    echo "Branch old_unity already exists locally, deleting it first..."
    git branch -D old_unity
fi

git checkout -b old_unity
if [ "$DRY_RUN" = true ]; then
    echo "[DRY-RUN] Would push: git push origin old_unity"
else
    git push origin old_unity -f
fi
echo "✓ old_unity branch created$([ "$DRY_RUN" = true ] && echo " (dry-run)" || echo " and pushed")"

echo ""
echo "Step 2: Renaming console_dev to old_console..."
git checkout console_dev

# Check if old_console already exists locally
if git show-ref --verify --quiet refs/heads/old_console; then
    echo "Branch old_console already exists locally, deleting it first..."
    git branch -D old_console
fi

git branch -m old_console
if [ "$DRY_RUN" = true ]; then
    echo "[DRY-RUN] Would push: git push origin old_console"
    echo "[DRY-RUN] Would delete: git push origin --delete console_dev"
else
    git push origin old_console -f
    git push origin --delete console_dev
fi
echo "✓ console_dev renamed to old_console$([ "$DRY_RUN" = true ] && echo " (dry-run)" || echo "")"

echo ""
echo "Step 3: Updating main to match dev..."
git checkout main
git reset --hard dev
if [ "$DRY_RUN" = true ]; then
    echo "[DRY-RUN] Would force push: git push origin main --force"
else
    git push origin main --force
fi
echo "✓ main branch updated with dev contents$([ "$DRY_RUN" = true ] && echo " (dry-run)" || echo "")"

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
