param(
    [string]$p1 = "Debug"
)

dotnet restore
dotnet build ".\src\" -c $p1
dotnet build ".\test\" -c $p1
