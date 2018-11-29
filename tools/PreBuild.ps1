param(
	[Parameter(Mandatory=$true)]
	[string]$solution,

	[Parameter(Mandatory=$true)]
	[string]$project,

	[Parameter(Mandatory=$true)]
	[string]$targetDir,
  
  [Parameter(Mandatory=$true)]
	[string]$targetFileName
)

$binary = Join-Path $solution tools\sigcheck64.exe
& $binary -nobanner -n $targetDir$targetFileName | Tee-Object -Variable version
$version = $version -replace ".$"

$path = Join-Path $solution appveyor.yml
$path2 = Join-Path $solution appveyor1.yml

if (Test-Path $path2) {
  Remove-Item $path2
}

.{
    "version: " + $version + "{build}"
    Get-Content $path | Select-Object -Skip 1
} |
Set-Content $path2

move-Item $path2 $path -Force