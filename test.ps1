dotnet test .\test\Tests.csproj
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }