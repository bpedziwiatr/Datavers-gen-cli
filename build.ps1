function main() {
    $currentPath = Get-Location
    rm .\publish
    MSBuild DataverseGen.Cli/DataverseGen.Cli.csproj /t:Build /p:OutDir=$currentPath\publish /p:Configuration=Release /p:DebugType=None
    
    createTemplateDirectories
    cd $currentPath
    copyTemplates
}
function copyTemplates() {
    Write-Host $path

    Copy-item * -destination c:\temp\targetdirectory -recurse`
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\Main\"  $currentPath"/publish/Templates/ExampleTemplate/" *.sbncs /s
    robocopy $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\TS-Main\"  $currentPath"/publish/Templates/ExampleTSTemplate/" *.sbncs /s
    #Copy-item -Force -Recurse -Verbose $sourceDirectory -Destination $destinationDirectory
    #     Copy-Item -Force  -Path $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\Main\"  -Recurse -Destination $currentPath"publish/Templates/ExampleTemplate/"
    # Copy-Item  -Path $currentPath"\DataverseGen.Core\Generators\Scriban\Templates\Multiple\TS-Main\"  -Recurse -Destination $currentPath"publish/Templates/ExampleTemplateTS/"
}
function createTemplateDirectories() {
    cd publish
    md Templates
    cd Templates
    md ExampleTemplate
    md ExampleTsTemplate
}
main