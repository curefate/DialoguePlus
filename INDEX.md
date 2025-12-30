# DialoguePlus Branch Reorganization - Documentation Index

## Quick Links

- 🚀 **[Quick Start Guide](QUICK_START.md)** - Start here for immediate action
- 📖 **[PR Overview](README_PR.md)** - Understand what this PR does
- 📊 **[Visual Guide](VISUAL_GUIDE.md)** - See before/after diagrams
- 📋 **[Detailed Instructions](BRANCH_REORGANIZATION.md)** - Step-by-step manual commands
- 📝 **[Summary](REORGANIZATION_SUMMARY.md)** - Overview and rationale

## The Goal

Reorganize repository branches to:
1. Archive `main` (Unity project) → `old_unity`
2. Rename `console_dev` → `old_console`
3. Make `dev` (C# console app) the new `main`

## Choose Your Path

### Path 1: I want to do this quickly ⚡
→ Go to [QUICK_START.md](QUICK_START.md)

### Path 2: I want to understand first 🤔
→ Go to [README_PR.md](README_PR.md)

### Path 3: I want to see what changes 👀
→ Go to [VISUAL_GUIDE.md](VISUAL_GUIDE.md)

### Path 4: I want detailed manual steps 📖
→ Go to [BRANCH_REORGANIZATION.md](BRANCH_REORGANIZATION.md)

## Files Overview

| File | Purpose | Audience |
|------|---------|----------|
| `reorganize_branches.sh` | Executable script | Admins ready to execute |
| `QUICK_START.md` | Fast reference | Experienced Git users |
| `README_PR.md` | Complete PR guide | First-time readers |
| `VISUAL_GUIDE.md` | Diagrams and visuals | Visual learners |
| `BRANCH_REORGANIZATION.md` | Detailed docs | Those wanting full details |
| `REORGANIZATION_SUMMARY.md` | High-level overview | Decision makers |
| `INDEX.md` | This file | Starting point |

## Execution Requirements

✅ **Required:**
- Git installed and configured
- Push access to curefate/DialoguePlus
- Administrator permissions on the repository

⚠️ **Important:**
- This cannot be automated due to GitHub security policies
- Must be executed manually by a repository admin
- Uses `--force` push to update main branch

## Safety Features

The provided script includes:
- ✅ Dry-run mode for testing
- ✅ Confirmation prompts
- ✅ Error handling
- ✅ Git repository validation
- ✅ No data loss (everything is archived)

## Support

If you encounter issues:
1. Check [BRANCH_REORGANIZATION.md](BRANCH_REORGANIZATION.md#troubleshooting)
2. Review error messages carefully
3. Ensure you have the required permissions
4. Try dry-run mode first: `./reorganize_branches.sh --dry-run`

## Quick Command Reference

```bash
# Test the changes (no modifications)
./reorganize_branches.sh --dry-run

# Execute the reorganization (requires confirmation)
./reorganize_branches.sh

# Verify afterwards
git fetch --all
git branch -r
```

---

**Ready to start?** → [QUICK_START.md](QUICK_START.md)

**Want more context?** → [README_PR.md](README_PR.md)
