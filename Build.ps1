$majorVersionNumber = "1"
$versionPrefix = "$majorVersionNumber.0.0"
$versionSuffix =
    If ($null -eq $env:APPVEYOR_BUILD_NUMBER) {"local"}
    Else {[string]::Format("build-{0:D4}", [System.Int32]::Parse($env:APPVEYOR_BUILD_NUMBER))}
$version = "$versionPrefix-$versionSuffix"
$assemblyVersion = "$majorVersionNumber.0.0.0"
$fileVersion =
    If ($null -eq $env:APPVEYOR_BUILD_NUMBER) {"$versionPrefix.0"}
    Else {[string]::Format("$versionPrefix.{0:D4}", [System.Int32]::Parse($env:APPVEYOR_BUILD_NUMBER))}
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
