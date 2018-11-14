Remove-Item -Path .\artifacts -Recurse -ErrorAction Ignore
$buildConfiguration = "Release"
dotnet clean -c $buildConfiguration .
dotnet build -c $buildConfiguration .
dotnet test -c $buildConfiguration .\Lognostics.Tests
$versionSuffix =
    If ($null -eq $env:APPVEYOR_BUILD_NUMBER) {"local"} Else {[System.Int32]::Parse($env:APPVEYOR_BUILD_NUMBER).ToString("0000")}
dotnet pack -c $buildConfiguration --version-suffix $versionSuffix -o ..\artifacts .\Lognostics