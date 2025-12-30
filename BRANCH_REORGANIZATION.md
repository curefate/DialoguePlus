# Branch Reorganization Instructions

This document contains the git commands needed to reorganize the branches in the DialoguePlus repository.

## Required Changes

1. Archive the current `main` branch to a new branch named `old_unity`
2. Rename the `console_dev` branch to `old_console`
3. Replace the contents of `main` with the contents from the `dev` branch

## Git Commands to Execute

**IMPORTANT**: These commands should be executed by someone with push access to the repository.

### Step 1: Create old_unity branch from current main

```bash
# Create and push the old_unity branch
git fetch origin main:main
git checkout main
git checkout -b old_unity
git push origin old_unity
```

### Step 2: Rename console_dev to old_console

```bash
# Rename the branch locally and on remote
git fetch origin console_dev:console_dev
git checkout console_dev
git branch -m old_console
git push origin old_console
git push origin --delete console_dev
```

### Step 3: Update main to match dev

```bash
# Replace main with dev contents
git fetch origin dev:dev
git checkout main
git reset --hard dev
git push origin main --force
```

### Step 4: Update default branch (Optional)

If you want to make `main` the default branch in GitHub (which it likely already is), you can:
1. Go to the repository Settings on GitHub
2. Navigate to Branches
3. Set `main` as the default branch

### Step 5: Clean up (Optional)

After verifying everything works, you can optionally delete the `dev` branch if it's no longer needed:

```bash
git push origin --delete dev
```

## Verification

After running these commands, verify the branches:

```bash
git fetch --all
git branch -r
```

You should see:
- `origin/main` - containing the contents from the old `dev` branch
- `origin/old_unity` - containing the original Unity project
- `origin/old_console` - containing the old console version

## Alternative: Single Script

Here's a complete script that does all the operations:

```bash
#!/bin/bash

# Branch Reorganization Script for DialoguePlus
set -e

echo "Fetching all branches..."
git fetch --all

echo "Step 1: Creating old_unity from main..."
git checkout main
git checkout -b old_unity
git push origin old_unity

echo "Step 2: Renaming console_dev to old_console..."
git checkout console_dev
git branch -m old_console
git push origin old_console
git push origin --delete console_dev

echo "Step 3: Updating main to match dev..."
git checkout main
git reset --hard dev
git push origin main --force

echo "Branch reorganization complete!"
echo "Verifying branches..."
git branch -r | grep -E "origin/(main|old_unity|old_console|dev)"
```

Save this script as `reorganize_branches.sh`, make it executable with `chmod +x reorganize_branches.sh`, and run it with `./reorganize_branches.sh`.

## Safety Notes

- **IMPORTANT**: The `git push --force` command in Step 3 will overwrite the `main` branch. Make sure this is what you want!
- All original content from `main` will be preserved in the `old_unity` branch
- All original content from `console_dev` will be preserved in the `old_console` branch
- Consider informing team members before making these changes
- Make sure you have a backup or can access the repository history if needed
