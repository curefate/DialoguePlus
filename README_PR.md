# Branch Reorganization PR

## What This PR Does

This Pull Request provides tools and documentation to reorganize the branches in the DialoguePlus repository as requested:

1. **Archive `main` to `old_unity`** - Preserves the Unity project
2. **Rename `console_dev` to `old_console`** - Keeps old console version  
3. **Update `main` with `dev` contents** - Makes the C# console app the default

## Why Manual Execution is Required

GitHub security policies prevent automated systems from:
- Creating new remote branches
- Renaming or deleting remote branches
- Force-pushing to branches

Therefore, a repository administrator must execute these changes manually.

## How to Use

### Quick Start (Recommended)

1. **Clone or pull the repository:**
   ```bash
   git clone https://github.com/curefate/DialoguePlus.git
   cd DialoguePlus
   ```

2. **Test the changes first (dry-run):**
   ```bash
   ./reorganize_branches.sh --dry-run
   ```

3. **Execute the reorganization:**
   ```bash
   ./reorganize_branches.sh
   ```
   Type `yes` when prompted to confirm.

4. **Verify the results:**
   ```bash
   git fetch --all
   git branch -r
   ```

### Manual Execution

If you prefer to run commands manually, see [BRANCH_REORGANIZATION.md](BRANCH_REORGANIZATION.md) for step-by-step instructions.

## What Gets Changed

### Current State
```
main            → Unity project (Assets/, show.gif)
console_dev     → Old C# console application
dev             → Current C# console application
```

### After Reorganization
```
main            → Current C# console application (from dev)
old_unity       → Unity project (archived from main)
old_console     → Old C# console application (renamed from console_dev)
dev             → Unchanged (can be deleted if desired)
```

## Files in This PR

| File | Purpose |
|------|---------|
| `reorganize_branches.sh` | Main executable script with safety features |
| `QUICK_START.md` | Quick reference guide |
| `BRANCH_REORGANIZATION.md` | Detailed documentation with manual commands |
| `REORGANIZATION_SUMMARY.md` | Overview and explanation |
| `README_PR.md` | This file |

## Safety Features

The script includes:
- ✅ Confirmation prompt before making changes
- ✅ Dry-run mode (`--dry-run`) to preview changes
- ✅ Git repository validation
- ✅ Clear progress messages
- ✅ Error handling (exits on any error)

## Important Notes

⚠️ **Branch Protection**: If `main` is a protected branch, you may need to:
- Temporarily disable branch protection in GitHub Settings → Branches
- Or perform the operations through the GitHub web interface

⚠️ **Force Push**: The script uses `git push --force` to update `main`. This is intentional and necessary, but ensure you understand the implications.

⚠️ **Backup**: All original content is preserved:
- Original `main` → `old_unity`
- Original `console_dev` → `old_console`

## Troubleshooting

### Authentication Errors
- Ensure you have push access to the repository
- Verify your Git credentials are configured
- Try using SSH instead of HTTPS if needed

### Branch Already Exists
If `old_unity` or `old_console` already exist:
```bash
git push origin --delete old_unity
git push origin --delete old_console
```

### Protected Branch Errors
Temporarily disable branch protection in:
GitHub Settings → Branches → Branch protection rules

## After Execution

1. **Verify branches:**
   ```bash
   git branch -r
   ```

2. **Check content:**
   ```bash
   git checkout main
   ls -la  # Should show C# console app files
   ```

3. **Optional cleanup:**
   ```bash
   # Delete dev branch if no longer needed
   git push origin --delete dev
   ```

4. **Inform team members** about the branch reorganization

## Questions or Issues?

If you encounter any problems:
1. Check the detailed documentation in `BRANCH_REORGANIZATION.md`
2. Review the troubleshooting section above
3. Open an issue in the repository

## License

This reorganization maintains all existing code and licenses. No code is deleted, only reorganized into different branches.
