$majorVersionNumber = "0"
$versionPrefix = "$majorVersionNumber.1.0"

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

Write-Output "Version: $version"
Write-Output "AssemblyVersion: $assemblyVersion"
Write-Output "FileVersion: $fileVersion"

Remove-Item -Path .\artifacts -Recurse -ErrorAction Ignore

$buildConfiguration = "Release"

dotnet clean . -c $buildConfiguration

if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }

dotnet build . -c $buildConfiguration `
    -p:Version=$version `
    -p:AssemblyVersion=$assemblyVersion `
    -p:FileVersion=$fileVersion

if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }

dotnet test .\Subliminal.Tests -c $buildConfiguration --no-build

if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }

dotnet pack .\Subliminal -c $buildConfiguration --no-build -o ..\artifacts `
    -p:PackageVersion=$version

if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
