# Quick Start Guide

## Branch Reorganization - Quick Reference

### Prerequisites
- Git installed and configured
- Push access to the curefate/DialoguePlus repository
- Current directory is the DialoguePlus repository root

### Execute the Reorganization

**Option 1: One-Click Script (Recommended)**
```bash
# Test first with dry-run
./reorganize_branches.sh --dry-run

# Execute the actual reorganization
./reorganize_branches.sh
```

**Option 2: Manual Commands**

```bash
# 1. Archive main to old_unity
git checkout main
git checkout -b old_unity
git push origin old_unity

# 2. Rename console_dev to old_console  
git checkout console_dev
git branch -m old_console
git push origin old_console
git push origin --delete console_dev

# 3. Update main with dev contents
git checkout main
git reset --hard dev
git push origin main --force
```

### Verify
```bash
git fetch --all
git branch -r
```

Should show:
- `origin/main` (updated with dev content)
- `origin/old_unity` (archived main)
- `origin/old_console` (renamed console_dev)

### Troubleshooting

**"Authentication failed"**
- Ensure you have push access to the repository
- Check your Git credentials are configured correctly
- You may need to use SSH instead of HTTPS

**"Updates were rejected"**
- The main branch may be protected
- You may need to temporarily disable branch protection in GitHub Settings → Branches
- Or use the GitHub web interface to perform these operations

**"Branch already exists"**
- If `old_unity` or `old_console` already exist, either:
  - Delete them first: `git push origin --delete old_unity`
  - Or use different branch names

### More Information
See `BRANCH_REORGANIZATION.md` for detailed documentation.
