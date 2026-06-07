#Requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$EntityName
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$OldPascalSingular = 'Example'
$OldPascalPlural   = 'Examples'
$OldCamelSingular  = 'example'
$OldCamelPlural    = 'examples'

# ── Validate PascalCase ────────────────────────────────────────────────────────
if ($EntityName -notmatch '^[A-Z][a-zA-Z0-9]+$') {
    Write-Host "ERROR: -EntityName must be PascalCase singular (e.g. 'StockPrice'). Got: '$EntityName'" -ForegroundColor Red
    exit 1
}

# ── Guard: rename-template must have been run first ────────────────────────────
$apiTemplateInName = Get-ChildItem -Recurse |
    Where-Object {
        $_.Name -like '*ApiTemplate*' -and
        $_.FullName -notmatch "(^|[\\/])\.git([\\/]|$)" -and
        $_.FullName -notmatch "(^|[\\/])bin([\\/]|$)" -and
        $_.FullName -notmatch "(^|[\\/])obj([\\/]|$)"
    } |
    Select-Object -First 1

if ($apiTemplateInName) {
    Write-Host "ERROR: 'ApiTemplate' still found in filename: $($apiTemplateInName.FullName)" -ForegroundColor Red
    Write-Host "       Run scripts/rename-template.ps1 first." -ForegroundColor Red
    exit 1
}

# Directories excluded from all search and replace operations
$ExcludedDirs = @('.git', 'bin', 'obj', '.vs', '.idea', 'node_modules', 'scripts')

function ShouldExclude([string]$path) {
    foreach ($dir in $ExcludedDirs) {
        $escaped = [regex]::Escape($dir)
        if ($path -match "(^|[\\/])$escaped([\\/]|$)") { return $true }
    }
    return $false
}

# Filenames that are project-scaffolding conventions, not entity-derived names.
# '.env.example' contains the substring 'example', so the rename pass below
# would otherwise rename it to e.g. '.env.product' and break the documented
# 'cp .env.example .env' setup flow.
$ConventionFilePatterns = @('.env*.example')

function IsConventionFile([string]$name) {
    foreach ($pattern in $ConventionFilePatterns) {
        if ($name -like $pattern) { return $true }
    }
    return $false
}

# ── Guard: Example must still exist in the codebase ────────────────────────────
$foundInName = Get-ChildItem -Recurse |
    Where-Object { $_.Name -like "*$OldPascalSingular*" -and -not (ShouldExclude $_.FullName) } |
    Select-Object -First 1

$foundInContent = $false
if (-not $foundInName) {
    :searchLoop foreach ($file in (Get-ChildItem -Recurse -File | Where-Object { -not (ShouldExclude $_.FullName) })) {
        try {
            if ([System.IO.File]::ReadAllText($file.FullName) -match $OldPascalSingular) {
                $foundInContent = $true
                break searchLoop
            }
        } catch {}
    }
}

if (-not $foundInName -and -not $foundInContent) {
    Write-Host "ERROR: '$OldPascalSingular' entity no longer found in codebase. Nothing to rename." -ForegroundColor Red
    exit 1
}

# ── Derive the 4 replacement forms ─────────────────────────────────────────────
$NewPascalSingular = $EntityName
$NewPascalPlural   = $EntityName + 's'
$NewCamelSingular  = $EntityName.Substring(0, 1).ToLower() + $EntityName.Substring(1)
$NewCamelPlural    = $NewPascalPlural.Substring(0, 1).ToLower() + $NewPascalPlural.Substring(1)

Write-Host ""
Write-Host "Derived forms:" -ForegroundColor Cyan
Write-Host ("  {0,-20} → {1}" -f $OldPascalSingular, $NewPascalSingular)
Write-Host ("  {0,-20} → {1}" -f $OldPascalPlural,   $NewPascalPlural)
Write-Host ("  {0,-20} → {1}" -f $OldCamelSingular,  $NewCamelSingular)
Write-Host ("  {0,-20} → {1}" -f $OldCamelPlural,    $NewCamelPlural)
Write-Host ""

$confirmPlural = Read-Host "Derived plural: '$NewPascalPlural'. Accept? (Y/n)"
if ($confirmPlural -match '^[Nn]') {
    $customPlural = Read-Host "Enter custom PascalCase plural (e.g. Indices)"
    if ($customPlural -notmatch '^[A-Z][a-zA-Z0-9]+$') {
        Write-Host "ERROR: Custom plural must be PascalCase (e.g. 'Indices')." -ForegroundColor Red
        exit 1
    }
    $OldPascalPlural = 'Examples'   # keep old plural as-is
    $OldCamelPlural  = 'examples'
    $NewPascalPlural = $customPlural
    $NewCamelPlural  = $customPlural.Substring(0, 1).ToLower() + $customPlural.Substring(1)

    Write-Host ""
    Write-Host "Using custom plural forms:" -ForegroundColor Cyan
    Write-Host ("  {0,-20} → {1}" -f $OldPascalPlural, $NewPascalPlural)
    Write-Host ("  {0,-20} → {1}" -f $OldCamelPlural,  $NewCamelPlural)
    Write-Host ""
}

# Build replacements sorted by source length descending (longest match first to
# prevent 'Examples' from being partially matched by the 'Example' rule first).
$Replacements = @(
    [PSCustomObject]@{ Old = $OldPascalPlural;   New = $NewPascalPlural   }
    [PSCustomObject]@{ Old = $OldCamelPlural;    New = $NewCamelPlural    }
    [PSCustomObject]@{ Old = $OldPascalSingular; New = $NewPascalSingular }
    [PSCustomObject]@{ Old = $OldCamelSingular;  New = $NewCamelSingular  }
) | Sort-Object { $_.Old.Length } -Descending

$DirsRenamed    = 0
$FilesRenamed   = 0
$ContentUpdated = 0

# ── Step 1: Update file contents ───────────────────────────────────────────────
Write-Host "  [1/3] Updating file contents..."

$allFiles = Get-ChildItem -Recurse -File | Where-Object { -not (ShouldExclude $_.FullName) }

foreach ($file in $allFiles) {
    try {
        $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
        if ($bytes.Length -eq 0) { continue }

        $checkLen = [Math]::Min(512, $bytes.Length)
        $isBinary = ($bytes[0..($checkLen - 1)] -contains [byte]0)
        if ($isBinary) { continue }

        $content = [System.Text.Encoding]::UTF8.GetString($bytes)

        $hasMatch = $false
        foreach ($r in $Replacements) {
            if ($content.Contains($r.Old)) { $hasMatch = $true; break }
        }

        if ($hasMatch) {
            foreach ($r in $Replacements) {
                $content = $content.Replace($r.Old, $r.New)
            }
            [System.IO.File]::WriteAllBytes($file.FullName, [System.Text.Encoding]::UTF8.GetBytes($content))
            $ContentUpdated++
        }
    }
    catch {
        # Skip unreadable / binary files
    }
}

# ── Step 2: Rename files ────────────────────────────────────────────────────────
Write-Host "  [2/3] Renaming files..."

$filesToRename = Get-ChildItem -Recurse -File | Where-Object {
    $name = $_.Name
    $hit  = $false
    foreach ($r in $Replacements) { if ($name -like "*$($r.Old)*") { $hit = $true; break } }
    $hit -and -not (ShouldExclude $_.FullName) -and -not (IsConventionFile $name)
}

foreach ($file in $filesToRename) {
    $newFileName = $file.Name
    foreach ($r in $Replacements) { $newFileName = $newFileName.Replace($r.Old, $r.New) }
    if ($newFileName -ne $file.Name) {
        Rename-Item -Path $file.FullName -NewName $newFileName
        $FilesRenamed++
    }
}

# ── Step 3: Rename directories (deepest first) ─────────────────────────────────
Write-Host "  [3/3] Renaming directories..."

$dirsToRename = Get-ChildItem -Recurse -Directory | Where-Object {
    $name = $_.Name
    $hit  = $false
    foreach ($r in $Replacements) { if ($name -like "*$($r.Old)*") { $hit = $true; break } }
    $hit -and -not (ShouldExclude $_.FullName)
} | Sort-Object { $_.FullName.Length } -Descending

foreach ($dir in $dirsToRename) {
    $newDirName = $dir.Name
    foreach ($r in $Replacements) { $newDirName = $newDirName.Replace($r.Old, $r.New) }
    if ($newDirName -ne $dir.Name) {
        Rename-Item -Path $dir.FullName -NewName $newDirName
        $DirsRenamed++
    }
}

# ── Route-string warning ────────────────────────────────────────────────────────
# Only relevant for multi-word entities where camelPlural differs from kebab-plural
$KebabPlural = ($NewCamelPlural -creplace '([A-Z])', '-$1').ToLower().TrimStart('-')

if ($KebabPlural -ne $NewCamelPlural) {
    # Scan controller files for the updated camelCase route
    $routeMatch = Get-ChildItem -Recurse -Filter '*.cs' |
        Where-Object { -not (ShouldExclude $_.FullName) } |
        Select-String -Pattern "\[Route\([^)]*$([regex]::Escape($NewCamelPlural))[^)]*\)\]" -SimpleMatch:$false |
        Select-Object -First 1

    if ($routeMatch) {
        Write-Host ""
        Write-Host "WARNING: Route strings updated to '$NewCamelPlural'. REST convention for multi-word" -ForegroundColor Yellow
        Write-Host "         resources is kebab-case ('$KebabPlural'). Review Controllers/V1/ and update manually if needed." -ForegroundColor Yellow
    }
}

# ── Summary ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "  Entity renamed    : $OldPascalSingular → $NewPascalSingular" -ForegroundColor Green
Write-Host "  Files renamed     : $FilesRenamed"
Write-Host "  Directories       : $DirsRenamed"
Write-Host "  Content updated   : $ContentUpdated files"
Write-Host ""
Write-Host "Next step: run 'make migrate Name=Initial$NewPascalSingular' to create your first migration." -ForegroundColor Yellow
Write-Host ""
