# Current Approach for Ubuntu

1. PUBLISH the solution for linux and output to the artifact folder

```powershell
dotnet publish .\cangulo.build\cangulo.build.csproj  -o .\artifacts\ -r linux-x64 --self-contained
```

2. Execute the cangulo.build from the other repository as next:

```powershell
./nuke-builder/cangulo.build --root . --target GenericRequestHandler --requestJSON '{ \"requestModel\": \"ExecuteAllUnitTestsInTheRepository\",\"originator\": \"originator\"}'
```

# TODO
1. Check if any parameter from the build command should be added to the publish

```powershell
dotnet build .\cangulo.build\cangulo.build.csproj /nodeReuse:false /p:UseSharedCompilation=false -nologo -clp:NoSummary --verbosity quiet -o .\artifacts\
```

# Include the code in a GitHub Action

https://docs.github.com/en/actions/creating-actions/creating-a-composite-run-steps-action

# Other Notes

1. Check the commands in the ps1 files

dotnet build .\cangulo.build\cangulo.build.csproj /nodeReuse:false /p:UseSharedCompilation=false -nologo -clp:NoSummary --verbosity quiet
dotnet run --project .\cangulo.build\cangulo.build.csproj --no-build
dotnet publish  -r win-x64 .\cangulo.build\cangulo.build.csproj -c Release /p:PublishSingleFile=true --self-contained -o ./artifacts

# Run in other solutions

dotnet run --project .\nuke-runner\cangulo.build\cangulo.build.csproj --no-build --root . --target GenericRequestHandler --requestJSON '{ \"requestModel\": \"ExecuteAllUnitTestsInTheRepository\",\"originator\": \"originator\"}'

--target GenericRequestHandler --requestJSON 

# Links
https://github.com/cangulo/cangulo.build
https://dotnetcoretutorials.com/2019/06/20/publishing-a-single-exe-file-in-net-core-3-0/
https://docs.microsoft.com/es-es/dotnet/core/tools/dotnet-publish
https://docs.microsoft.com/es-es/dotnet/core/deploying/deploy-with-cli
https://docs.microsoft.com/es-es/dotnet/core/rid-catalog
https://docs.microsoft.com/es-es/dotnet/core/tools/dotnet-run
https://github.com/cangulo/cangulo.common