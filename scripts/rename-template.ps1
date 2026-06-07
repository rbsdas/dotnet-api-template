#Requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$NewName
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$OldName = 'ApiTemplate'

# Validate PascalCase
if ($NewName -notmatch '^[A-Z][a-zA-Z0-9]+$') {
    Write-Host "ERROR: -NewName must be PascalCase (e.g. 'TaskManagerApi'). Got: '$NewName'" -ForegroundColor Red
    exit 1
}

if ($NewName -eq $OldName) {
    Write-Host "ERROR: -NewName is the same as the current name ('$OldName'). Nothing to do." -ForegroundColor Red
    exit 1
}

# Must run from repo root
if (-not (Test-Path (Join-Path (Get-Location) "$OldName.sln"))) {
    Write-Host "ERROR: Must be run from the repository root ($OldName.sln not found in: $(Get-Location))." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Renaming '$OldName' → '$NewName'..." -ForegroundColor Cyan
Write-Host ""

# Directories excluded from all operations
$ExcludedDirs = @('.git', 'bin', 'obj', '.vs', '.idea', 'node_modules', 'scripts')

function ShouldExclude([string]$path) {
    foreach ($dir in $ExcludedDirs) {
        $escaped = [regex]::Escape($dir)
        if ($path -match "(^|[\\/])$escaped([\\/]|$)") { return $true }
    }
    return $false
}

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

        # Skip binary files: look for null bytes in the first 512 bytes
        $checkLen = [Math]::Min(512, $bytes.Length)
        $isBinary = ($bytes[0..($checkLen - 1)] -contains [byte]0)
        if ($isBinary) { continue }

        $content = [System.Text.Encoding]::UTF8.GetString($bytes)
        if ($content.Contains($OldName)) {
            $updated = $content.Replace($OldName, $NewName)
            [System.IO.File]::WriteAllBytes($file.FullName, [System.Text.Encoding]::UTF8.GetBytes($updated))
            $ContentUpdated++
        }
    }
    catch {
        # Skip files that cannot be read as UTF-8 text
    }
}

# ── Step 2: Rename files ────────────────────────────────────────────────────────
Write-Host "  [2/3] Renaming files..."

$filesToRename = Get-ChildItem -Recurse -File |
    Where-Object { $_.Name -like "*$OldName*" -and -not (ShouldExclude $_.FullName) }

foreach ($file in $filesToRename) {
    $newFileName = $file.Name.Replace($OldName, $NewName)
    Rename-Item -Path $file.FullName -NewName $newFileName
    $FilesRenamed++
}

# ── Step 3: Rename directories (deepest first to avoid stale paths) ────────────
Write-Host "  [3/3] Renaming directories..."

$dirsToRename = Get-ChildItem -Recurse -Directory |
    Where-Object { $_.Name -like "*$OldName*" -and -not (ShouldExclude $_.FullName) } |
    Sort-Object { $_.FullName.Length } -Descending

foreach ($dir in $dirsToRename) {
    $newDirName = $dir.Name.Replace($OldName, $NewName)
    Rename-Item -Path $dir.FullName -NewName $newDirName
    $DirsRenamed++
}

# ── Summary ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "Done!" -ForegroundColor Green
Write-Host ""
Write-Host "  Directories renamed : $DirsRenamed"
Write-Host "  Files renamed       : $FilesRenamed"
Write-Host "  Content updated     : $ContentUpdated files"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. cp .env.example .env   (fill in your values)"
Write-Host "  2. make docker-up"
Write-Host ""
