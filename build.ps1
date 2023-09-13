function main() {
    $currentPath = Get-Location
    Remove-Item .\publish -Recurse -ErrorAction Ignore    
    dotnet publish "DataverseGen.Cli/DataverseGen.Cli.csproj" /p:DebugType=None /p:DebugSymbols=false -f "net7.0" -r win-x64 -p:PublishSingleFile=true --self-contained true -p:PublishReadyToRun=true -p:PublishDir=$currentPath\publish\bin
	
    Set-Location $currentPath
    copyTemplates
    MakeZip $currentPath
}
function copyTemplates() {
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\Main\"  $currentPath"/publish/bin/Templates/ExampleTemplate/" *.sbncs /s
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\TS-Main\"  $currentPath"/publish/bin/Templates/ExampleTSTemplate/" *.sbncs /s  
}
function getGitVersioion(){
    $gitv = dotnet-gitversion | ConvertFrom-JSON -ErrorAction stop
    return $gitv
}
function MakeZip([string]$currentPath){
    $gitv = getGitVersioion
    $compress = @{
        Path = $currentPath+"/publish/bin/**"
        CompressionLevel = "Fastest"
        DestinationPath = $currentPath+"/publish/DataverseGen-CLI_"+$gitv.NuGetVersionV2+".zip"
      }
      Compress-Archive @compress
}

main
