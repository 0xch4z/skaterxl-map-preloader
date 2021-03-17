param(
    [String]$TargetName = $(throw "-TargetName is required."), 
    [String]$ProjectDir = $(throw "-ProjectDir is required.")
)

$modBuildDir = $ProjectDir + "\bin\Debug\";
$modDistDir = $modBuildDir + $TargetName;
$modDistZip = $modBuildDir + $TargetName + ".zip";

$modLibName = $TargetName + ".dll";
$modLib = $modBuildDir + $modLibName;
$modDistLib = $modDistDir + "\" + $modLibName;
$modInfo = $ProjectDir + "\Info.json";
$modDistInfo = $modDistDir + "\Info.json";

New-Item -ItemType Directory -Force -Path $modDistDir;
Copy-Item -Path $modLib -Destination $modDistLib;
Copy-Item -Path $modInfo -Destination $modDistInfo -Force;

Compress-Archive -Path $modDistDir -DestinationPath $modDistZip -Force;
