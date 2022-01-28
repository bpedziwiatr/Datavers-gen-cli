function main() {
    $currentPath = Get-Location
    Remove-Item .\publish
    MSBuild DataverseGen.Cli/DataverseGen.Cli.csproj /t:Build /p:OutDir=$currentPath\publish\bin /p:Configuration=Release /p:DebugType=None
    
    Set-Location $currentPath
    copyTemplates
    MakeZip $currentPath
}
function copyTemplates() {
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\Main\"  $currentPath"/publish/bin/Templates/ExampleTemplate/" *.sbncs /s
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\TS-Main\"  $currentPath"/publish/bin/Templates/ExampleTSTemplate/" *.sbncs /s  
}
function getGitVersioion(){
    $gitv = gitversion | ConvertFrom-JSON -ErrorAction stop
    return $gitv
}
function MakeZip([string]$currentPath){
    $gitv = getGitVersioion
    $compress = @{
        Path = $currentPath+"/publish/bin/*.*"
        CompressionLevel = "Fastest"
        DestinationPath = $currentPath+"/publish/DataverseGen-CLI_"+$gitv.NuGetVersionV2+".zip"
      }
      Compress-Archive @compress
}

main