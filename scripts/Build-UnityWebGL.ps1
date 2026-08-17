[CmdletBinding()]
param(
    [string]$UnityEditorPath = $env:UNITY_EDITOR_PATH
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot "CSharpWars\CSharpWars.Frontend"

if ([string]::IsNullOrWhiteSpace($UnityEditorPath)) {
    $projectVersionPath = Join-Path $projectPath "ProjectSettings\ProjectVersion.txt"
    $versionMatch = Select-String -Path $projectVersionPath -Pattern "^m_EditorVersion:\s*(.+)$" |
        Select-Object -First 1

    if ($null -eq $versionMatch) {
        throw "Could not determine the Unity version from '$projectVersionPath'."
    }

    $unityVersion = $versionMatch.Matches[0].Groups[1].Value.Trim()
    $UnityEditorPath = Join-Path ${env:ProgramFiles} "Unity\Hub\Editor\$unityVersion\Editor\Unity.exe"
}

if (-not (Test-Path -LiteralPath $UnityEditorPath -PathType Leaf)) {
    throw "Unity Editor was not found at '$UnityEditorPath'. Set UNITY_EDITOR_PATH to Unity.exe."
}

& $UnityEditorPath `
    -batchmode `
    -quit `
    -projectPath $projectPath `
    -executeMethod CSharpWars.Editor.WebGlBuildAutomation.BuildAndStage `
    -logFile -

if ($LASTEXITCODE -ne 0) {
    throw "Unity WebGL build failed with exit code $LASTEXITCODE."
}

$webGlPath = Join-Path $repositoryRoot "CSharpWars\CSharpWars.Web\wwwroot\lib\unity"
Write-Host "Unity WebGL build copied to '$webGlPath'."
