$majorVersionNumber = "1"
$versionPrefix = "$majorVersionNumber.0.0"

Write-Output "APPVEYOR_BUILD_NUMBER: $env:APPVEYOR_BUILD_NUMBER"
Write-Output "APPVEYOR_REPO_COMMIT: $env:APPVEYOR_REPO_COMMIT"
Write-Output "APPVEYOR_REPO_TAG: $env:APPVEYOR_REPO_TAG"
Write-Output "APPVEYOR_REPO_TAG_NAME: $env:APPVEYOR_REPO_TAG_NAME"

$version =
    If ($env:APPVEYOR_REPO_TAG -eq "true" -And $env:APPVEYOR_REPO_TAG_NAME.StartsWith("v$versionPrefix"))
        {$env:APPVEYOR_REPO_TAG_NAME.Remove(0, 1)}
    ElseIf ($null -eq $env:APPVEYOR_BUILD_NUMBER) {"$versionPrefix-local"}
    Else {[string]::Format("$versionPrefix-build-{0:D4}", [System.Int32]::Parse($env:APPVEYOR_BUILD_NUMBER))}

$assemblyVersion = "$majorVersionNumber.0.0.0"

$fileVersion =
    If ($null -eq $env:APPVEYOR_BUILD_NUMBER) {"$versionPrefix.0"}
    Else {[string]::Format("$versionPrefix.{0}", [System.Int32]::Parse($env:APPVEYOR_BUILD_NUMBER))}

$informationalVersion =
    If ($null -eq $env:APPVEYOR_REPO_COMMIT) {"$version"}
    Else {[string]::Format("$version+{0}", $env:APPVEYOR_REPO_COMMIT.Substring(0, 7))}

Write-Output "Version: $version"
Write-Output "AssemblyVersion: $assemblyVersion"
Write-Output "FileVersion: $fileVersion"
Write-Output "InformationalVersion: $informationalVersion"

Remove-Item -Path .\artifacts -Recurse -ErrorAction Ignore

$buildConfiguration = "Release"

dotnet clean . -c $buildConfiguration

dotnet build . -c $buildConfiguration `
    -p:Version=$version `
    -p:AssemblyVersion=$assemblyVersion `
    -p:FileVersion=$fileVersion `
    -p:InformationalVersion=$informationalVersion

dotnet test .\Subliminal.Tests -c $buildConfiguration --no-build

dotnet pack .\Subliminal -c $buildConfiguration --no-build -o ..\artifacts `
    -p:PackageVersion=$version

if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
