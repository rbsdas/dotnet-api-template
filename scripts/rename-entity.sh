#!/usr/bin/env bash
# rename-entity.sh — renames the 'Example' placeholder entity to your first real domain entity.
# Usage: ./scripts/rename-entity.sh <EntityName>
# Example: ./scripts/rename-entity.sh StockPrice
set -euo pipefail

OLD_PASCAL_SINGULAR='Example'
OLD_PASCAL_PLURAL='Examples'
OLD_CAMEL_SINGULAR='example'
OLD_CAMEL_PLURAL='examples'

# ── Detect OS for sed -i behaviour ────────────────────────────────────────────
if uname -s | grep -qi darwin; then
    SED_INPLACE=(-i '')   # BSD sed (macOS)
else
    SED_INPLACE=(-i)      # GNU sed (Linux)
fi

# ── Usage / arg validation ─────────────────────────────────────────────────────
if [[ $# -ne 1 || -z "${1:-}" ]]; then
    echo "Usage: $(basename "$0") <EntityName>"
    echo "  EntityName must be PascalCase singular (e.g. StockPrice)"
    exit 1
fi

ENTITY_NAME="$1"

if [[ ! "$ENTITY_NAME" =~ ^[A-Z][a-zA-Z0-9]+$ ]]; then
    echo "ERROR: <EntityName> must be PascalCase singular (e.g. 'StockPrice'). Got: '$ENTITY_NAME'"
    exit 1
fi

# Common find prune expression
PRUNE_EXPR='( -name .git -o -name bin -o -name obj -o -name .vs -o -name .idea -o -name node_modules -o -name scripts )'

# ── Guard: rename-template must have been run first ────────────────────────────
if find . $PRUNE_EXPR -prune -o \( -name '*ApiTemplate*' \) -print -quit 2>/dev/null | grep -q .; then
    echo "ERROR: 'ApiTemplate' still found in filenames. Run scripts/rename-template.sh first."
    exit 1
fi

# ── Guard: Example must still exist in the codebase ────────────────────────────
FOUND_EXAMPLE=false

# Check filenames first (fast)
if find . $PRUNE_EXPR -prune -o -name "*${OLD_PASCAL_SINGULAR}*" -print -quit 2>/dev/null | grep -q .; then
    FOUND_EXAMPLE=true
fi

# Fall back to content search
if [[ "$FOUND_EXAMPLE" == "false" ]]; then
    if grep -rqIF "$OLD_PASCAL_SINGULAR" \
        --exclude-dir=.git --exclude-dir=bin --exclude-dir=obj \
        --exclude-dir=.vs --exclude-dir=.idea --exclude-dir=scripts \
        . 2>/dev/null; then
        FOUND_EXAMPLE=true
    fi
fi

if [[ "$FOUND_EXAMPLE" == "false" ]]; then
    echo "ERROR: '$OLD_PASCAL_SINGULAR' entity no longer found in codebase. Nothing to rename."
    exit 1
fi

# ── Derive the 4 replacement forms ─────────────────────────────────────────────
NEW_PASCAL_SINGULAR="$ENTITY_NAME"
NEW_PASCAL_PLURAL="${ENTITY_NAME}s"

# Lowercase first character for camelCase
_first="${ENTITY_NAME:0:1}"
_rest="${ENTITY_NAME:1}"
NEW_CAMEL_SINGULAR="$(echo "$_first" | tr '[:upper:]' '[:lower:]')$_rest"

_pfirst="${NEW_PASCAL_PLURAL:0:1}"
_prest="${NEW_PASCAL_PLURAL:1}"
NEW_CAMEL_PLURAL="$(echo "$_pfirst" | tr '[:upper:]' '[:lower:]')$_prest"

echo ""
echo "Derived forms:"
printf "  %-22s → %s\n" "$OLD_PASCAL_SINGULAR" "$NEW_PASCAL_SINGULAR"
printf "  %-22s → %s\n" "$OLD_PASCAL_PLURAL"   "$NEW_PASCAL_PLURAL"
printf "  %-22s → %s\n" "$OLD_CAMEL_SINGULAR"  "$NEW_CAMEL_SINGULAR"
printf "  %-22s → %s\n" "$OLD_CAMEL_PLURAL"    "$NEW_CAMEL_PLURAL"
echo ""

read -r -p "Derived plural: '$NEW_PASCAL_PLURAL'. Accept? (Y/n): " CONFIRM_PLURAL || true

if [[ "${CONFIRM_PLURAL:-Y}" =~ ^[Nn] ]]; then
    read -r -p "Enter custom PascalCase plural (e.g. Indices): " CUSTOM_PLURAL || true
    if [[ ! "${CUSTOM_PLURAL:-}" =~ ^[A-Z][a-zA-Z0-9]+$ ]]; then
        echo "ERROR: Custom plural must be PascalCase (e.g. 'Indices')."
        exit 1
    fi
    NEW_PASCAL_PLURAL="$CUSTOM_PLURAL"
    _cpfirst="${CUSTOM_PLURAL:0:1}"
    _cprest="${CUSTOM_PLURAL:1}"
    NEW_CAMEL_PLURAL="$(echo "$_cpfirst" | tr '[:upper:]' '[:lower:]')$_cprest"

    echo ""
    echo "Using custom plural forms:"
    printf "  %-22s → %s\n" "$OLD_PASCAL_PLURAL" "$NEW_PASCAL_PLURAL"
    printf "  %-22s → %s\n" "$OLD_CAMEL_PLURAL"  "$NEW_CAMEL_PLURAL"
    echo ""
fi

DIRS_RENAMED=0
FILES_RENAMED=0
CONTENT_UPDATED=0

# ── Step 1: Update file contents ───────────────────────────────────────────────
# Replacements are applied in longest-match-first order within a single sed call,
# so 'Examples' is replaced before 'Example' is considered (preventing double-apply).
echo "  [1/3] Updating file contents..."

while IFS= read -r -d '' file; do
    # grep -I skips binary files; -F is fixed-string; any match triggers sed
    if grep -qIF "$OLD_PASCAL_PLURAL"   "$file" 2>/dev/null || \
       grep -qIF "$OLD_CAMEL_PLURAL"    "$file" 2>/dev/null || \
       grep -qIF "$OLD_PASCAL_SINGULAR" "$file" 2>/dev/null || \
       grep -qIF "$OLD_CAMEL_SINGULAR"  "$file" 2>/dev/null; then
        sed "${SED_INPLACE[@]}" \
            -e "s/${OLD_PASCAL_PLURAL}/${NEW_PASCAL_PLURAL}/g"     \
            -e "s/${OLD_CAMEL_PLURAL}/${NEW_CAMEL_PLURAL}/g"       \
            -e "s/${OLD_PASCAL_SINGULAR}/${NEW_PASCAL_SINGULAR}/g" \
            -e "s/${OLD_CAMEL_SINGULAR}/${NEW_CAMEL_SINGULAR}/g"   \
            "$file"
        CONTENT_UPDATED=$((CONTENT_UPDATED + 1))
    fi
done < <(find . $PRUNE_EXPR -prune -o -type f -print0)

# ── Step 2: Rename files ────────────────────────────────────────────────────────
# Process in longest-match-first order to avoid partial renames.
echo "  [2/3] Renaming files..."

_rename_files() {
    local OLD_FORM="$1" NEW_FORM="$2"
    while IFS= read -r -d '' file; do
        dir="$(dirname "$file")"
        base="$(basename "$file")"
        new_base="${base//$OLD_FORM/$NEW_FORM}"
        if [[ "$new_base" != "$base" ]]; then
            mv -- "$file" "$dir/$new_base"
            FILES_RENAMED=$((FILES_RENAMED + 1))
        fi
    done < <(find . $PRUNE_EXPR -prune -o -type f -name "*${OLD_FORM}*" -print0)
}

_rename_files "$OLD_PASCAL_PLURAL"   "$NEW_PASCAL_PLURAL"
_rename_files "$OLD_CAMEL_PLURAL"    "$NEW_CAMEL_PLURAL"
_rename_files "$OLD_PASCAL_SINGULAR" "$NEW_PASCAL_SINGULAR"
_rename_files "$OLD_CAMEL_SINGULAR"  "$NEW_CAMEL_SINGULAR"

# ── Step 3: Rename directories (deepest first) ─────────────────────────────────
echo "  [3/3] Renaming directories..."

_rename_dirs() {
    local OLD_FORM="$1" NEW_FORM="$2"
    while IFS= read -r dir; do
        parent="$(dirname "$dir")"
        base="$(basename "$dir")"
        new_base="${base//$OLD_FORM/$NEW_FORM}"
        if [[ "$new_base" != "$base" ]]; then
            mv -- "$dir" "$parent/$new_base"
            DIRS_RENAMED=$((DIRS_RENAMED + 1))
        fi
    done < <(
        find . $PRUNE_EXPR -prune -o -type d -name "*${OLD_FORM}*" -print \
        | awk '{ print length, $0 }' \
        | sort -rn \
        | cut -d' ' -f2-
    )
}

_rename_dirs "$OLD_PASCAL_PLURAL"   "$NEW_PASCAL_PLURAL"
_rename_dirs "$OLD_CAMEL_PLURAL"    "$NEW_CAMEL_PLURAL"
_rename_dirs "$OLD_PASCAL_SINGULAR" "$NEW_PASCAL_SINGULAR"
_rename_dirs "$OLD_CAMEL_SINGULAR"  "$NEW_CAMEL_SINGULAR"

# ── Route-string warning ────────────────────────────────────────────────────────
# Only relevant for multi-word entities (e.g. StockPrice → stockPrices vs stock-prices)
KEBAB_PLURAL="$(echo "$NEW_CAMEL_PLURAL" | sed 's/\([A-Z]\)/-\1/g' | tr '[:upper:]' '[:lower:]' | sed 's/^-//')"

if [[ "$KEBAB_PLURAL" != "$NEW_CAMEL_PLURAL" ]]; then
    if grep -rqIF "$NEW_CAMEL_PLURAL" --include="*.cs" \
        --exclude-dir=.git --exclude-dir=bin --exclude-dir=obj --exclude-dir=scripts \
        . 2>/dev/null; then
        echo ""
        echo "WARNING: Route strings updated to '$NEW_CAMEL_PLURAL'. REST convention for multi-word"
        echo "         resources is kebab-case ('$KEBAB_PLURAL'). Review Controllers/V1/ and update manually if needed."
    fi
fi

# ── Summary ─────────────────────────────────────────────────────────────────────
echo ""
printf "  ✔ Entity renamed    : %s → %s\n" "$OLD_PASCAL_SINGULAR" "$NEW_PASCAL_SINGULAR"
printf "    Files renamed     : %d\n"       "$FILES_RENAMED"
printf "    Directories       : %d\n"       "$DIRS_RENAMED"
printf "    Content updated   : %d files\n" "$CONTENT_UPDATED"
echo ""
echo "Next step: run 'make migrate Name=Initial${NEW_PASCAL_SINGULAR}' to create your first migration."
echo ""
