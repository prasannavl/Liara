param (
    [int]$projectNumber = 0
 )

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$packageFileName = "Packages.txt";
$solutionFile = "Liara.sln";

$currentPath = $scriptPath;
Push-Location $scriptPath;

while (![System.IO.File]::Exists([System.IO.Path]::Combine($currentPath, $solutionFile)))
{
    cd ..
    $currentPath = (get-location)
}

$rootPath = $currentPath;
Push-Location $rootPath

$lineNumber = 0;

foreach ($line in [System.IO.File]::ReadLines([System.IO.Path]::Combine($scriptPath, $packageFileName)))
{
$lineNumber++;
if ($projectNumber -ne 0)
{
    if ($projectNumber -eq $lineNumber)
    {
        # Move into sources dirctory
        cd $line
        rm *.nupkg
        NuGet pack -prop Configuration=Release
        # Back to root path
        Pop-Location
        break;
    }
    else {

        continue;
    }
}

# Move into sources dirctory
cd $line
rm *.nupkg
NuGet pack -prop Configuration=Release
# Back to root path
Pop-Location

}

Pop-Location