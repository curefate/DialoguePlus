# Branch Reorganization Summary

## Overview

This PR provides the necessary scripts and documentation to reorganize the branches in the DialoguePlus repository according to the requirements:

1. **Archive main to old_unity**: The current `main` branch (Unity project) will be preserved as `old_unity`
2. **Rename console_dev to old_console**: The `console_dev` branch will be renamed to `old_console`
3. **Update main with dev contents**: The `main` branch will be updated to contain the C# console application from the `dev` branch

## Why Manual Execution is Required

Due to GitHub security constraints, automated scripts cannot:
- Create new branches in the remote repository
- Rename or delete remote branches
- Force push to protected branches

Therefore, these operations must be performed manually by a repository administrator with appropriate permissions.

## How to Execute

You have two options:

### Option 1: Use the Automated Script (Recommended)

```bash
cd /home/runner/work/DialoguePlus/DialoguePlus
./reorganize_branches.sh
```

This script will automatically perform all three steps.

### Option 2: Manual Execution

Follow the step-by-step instructions in `BRANCH_REORGANIZATION.md`.

## What This Changes

### Before:
- `main`: Unity project with Assets folder
- `console_dev`: Older C# console application
- `dev`: Current C# console application

### After:
- `main`: Current C# console application (from `dev`)
- `old_unity`: Unity project (archived from old `main`)
- `old_console`: Older C# console application (renamed from `console_dev`)
- `dev`: Unchanged (can be optionally deleted later)

## Files Included

1. **reorganize_branches.sh** - Executable bash script that performs all operations
2. **BRANCH_REORGANIZATION.md** - Detailed documentation with manual commands
3. **REORGANIZATION_SUMMARY.md** - This file, explaining the changes

## Safety Notes

⚠️ **Important**: 
- The reorganization will overwrite the `main` branch
- All original content is preserved in `old_unity` and `old_console` branches
- Consider informing team members before executing these changes
- The script uses `--force` push for the main branch update

## Verification

After running the script, verify the changes:

```bash
git fetch --all
git branch -r
```

Expected branches:
- `origin/main` - C# console application
- `origin/old_unity` - Unity project  
- `origin/old_console` - Old console version
- `origin/dev` - Same as new main (can be deleted)
