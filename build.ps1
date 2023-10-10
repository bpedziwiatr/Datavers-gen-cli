function main([bool]$clearPublish) {
	if ($selfContainer) {
		Remove-Item .\publish -Recurse -ErrorAction Ignore 
	}	
    $currentPath = Get-Location    
   buildPackage($true)
   buildPackage($false)
}
function buildPackage([bool]$selfContainer){
	Remove-Item .\publish\bin -Recurse -ErrorAction Ignore    
	dotnet publish "DataverseGen.Cli/DataverseGen.Cli.csproj" /p:DebugType=None /p:DebugSymbols=false -c 'Release' -f "net7.0" -r win-x64 -p:PublishSingleFile=true --self-contained $selfContainer -p:PublishReadyToRun=true -p:PublishDir=$currentPath\publish\bin	
    Set-Location $currentPath
    copyTemplates
    MakeZip $currentPath $selfContainer
}
function copyTemplates() {
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\Main\"  $currentPath"/publish/bin/Templates/ExampleTemplate/" *.sbncs /s
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\TS-Main\"  $currentPath"/publish/bin/Templates/ExampleTSTemplate/" *.sbncs /s  
}
function getGitVersioion(){
    $gitv = dotnet-gitversion | ConvertFrom-JSON -ErrorAction stop
    return $gitv
}
function MakeZip([string]$currentPath,[bool]$selfContainer){
    $gitv = getGitVersioion
	if ($selfContainer) {
		$containerName = "includedotnet"
	} else {
		$containerName = "nodotnet"
	}
    $compress = @{
        Path = $currentPath+"/publish/bin/**"
        CompressionLevel = "Fastest"
        DestinationPath = $currentPath+"/publish/DataverseGen-CLI_" +$containerName+"_"+$gitv.NuGetVersionV2+".zip"
      }
      Compress-Archive @compress
}

main
