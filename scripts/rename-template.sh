#!/usr/bin/env bash
# rename-template.sh — renames the 'ApiTemplate' placeholder throughout the project.
# Usage: ./scripts/rename-template.sh <NewName>
# Example: ./scripts/rename-template.sh TaskManagerApi
set -euo pipefail

OLD_NAME='ApiTemplate'

# ── Detect OS for sed -i behaviour ────────────────────────────────────────────
if uname -s | grep -qi darwin; then
    SED_INPLACE=(-i '')   # BSD sed (macOS)
else
    SED_INPLACE=(-i)      # GNU sed (Linux)
fi

# ── Usage / arg validation ─────────────────────────────────────────────────────
if [[ $# -ne 1 || -z "${1:-}" ]]; then
    echo "Usage: $(basename "$0") <NewName>"
    echo "  NewName must be PascalCase (e.g. TaskManagerApi)"
    exit 1
fi

NEW_NAME="$1"

if [[ ! "$NEW_NAME" =~ ^[A-Z][a-zA-Z0-9]+$ ]]; then
    echo "ERROR: <NewName> must be PascalCase (e.g. 'TaskManagerApi'). Got: '$NEW_NAME'"
    exit 1
fi

if [[ "$NEW_NAME" == "$OLD_NAME" ]]; then
    echo "ERROR: <NewName> is the same as the current name ('$OLD_NAME'). Nothing to do."
    exit 1
fi

# ── Must run from repo root ────────────────────────────────────────────────────
if [[ ! -f "${OLD_NAME}.sln" ]]; then
    echo "ERROR: Must be run from the repository root (${OLD_NAME}.sln not found in: $(pwd))."
    exit 1
fi

echo ""
echo "Renaming '$OLD_NAME' → '$NEW_NAME'..."
echo ""

DIRS_RENAMED=0
FILES_RENAMED=0
CONTENT_UPDATED=0

# Common find prune expression — excludes noisy / binary dirs
PRUNE_EXPR='( -name .git -o -name bin -o -name obj -o -name .vs -o -name .idea -o -name node_modules -o -name scripts )'

# ── Step 1: Update file contents ───────────────────────────────────────────────
echo "  [1/3] Updating file contents..."

while IFS= read -r -d '' file; do
    # grep -I treats binary files as non-matching; -F is fixed-string; -q is quiet
    if grep -qIF "$OLD_NAME" "$file" 2>/dev/null; then
        sed "${SED_INPLACE[@]}" "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
        CONTENT_UPDATED=$((CONTENT_UPDATED + 1))
    fi
done < <(find . $PRUNE_EXPR -prune -o -type f -print0)

# ── Step 2: Rename files ────────────────────────────────────────────────────────
echo "  [2/3] Renaming files..."

while IFS= read -r -d '' file; do
    dir="$(dirname "$file")"
    base="$(basename "$file")"
    new_base="${base//$OLD_NAME/$NEW_NAME}"
    if [[ "$new_base" != "$base" ]]; then
        mv -- "$file" "$dir/$new_base"
        FILES_RENAMED=$((FILES_RENAMED + 1))
    fi
done < <(find . $PRUNE_EXPR -prune -o -type f -name "*${OLD_NAME}*" -print0)

# ── Step 3: Rename directories (deepest first) ─────────────────────────────────
echo "  [3/3] Renaming directories..."

# Sort by path length descending so children are renamed before parents
while IFS= read -r dir; do
    parent="$(dirname "$dir")"
    base="$(basename "$dir")"
    new_base="${base//$OLD_NAME/$NEW_NAME}"
    if [[ "$new_base" != "$base" ]]; then
        mv -- "$dir" "$parent/$new_base"
        DIRS_RENAMED=$((DIRS_RENAMED + 1))
    fi
done < <(
    find . $PRUNE_EXPR -prune -o -type d -name "*${OLD_NAME}*" -print \
    | awk '{ print length, $0 }' \
    | sort -rn \
    | cut -d' ' -f2-
)

# ── Summary ─────────────────────────────────────────────────────────────────────
echo ""
echo "Done!"
echo ""
echo "  Directories renamed : $DIRS_RENAMED"
echo "  Files renamed       : $FILES_RENAMED"
echo "  Content updated     : $CONTENT_UPDATED files"
echo ""
echo "Next steps:"
echo "  1. cp .env.example .env   (fill in your values)"
echo "  2. make docker-up"
echo ""
