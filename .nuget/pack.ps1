#
# pack.ps1
#
$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\FluentCommandLineParser\bin\Release\FluentCommandLineParser.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\NuGet\Pingo.FluentCommandLineParser.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\nuget\Pingo.FluentCommandLineParser.compiled.nuspec

& $root\NuGet\NuGet.exe pack $root\nuget\Pingo.FluentCommandLineParser.compiled.nuspec