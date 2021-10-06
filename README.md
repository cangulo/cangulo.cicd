################################################
# DO NOT USE ME - THIS PROJECT IS NOT READY YET
################################################

# cangulo.cicd

## Goals

Main Goal:
* Offer a solution for all CICD operations as execute tests, create release, update changelog , along different repositories using NUKE
* Make it customizable  by providing settings in a cicd.json file
* List the result in a json format so can be used in any pipeline


## Available actions

| Action (Target)   | Group  | cicd.json properties required |
| ----------------- | ------ | ----------------------------- |
| CleanBuildFolders | dotnet |                               |
| Restore           | dotnet |                               |
| Compile           | dotnet |                               |
| ExecuteUnitTests  | dotnet |                               |
| Publish           | dotnet |                               |

## how does this work?

All the code behind is based on [NUKE](https://nuke.build). Following their approach, any CICD action will be defined as _Target_ in the build.cs, they are defined in a group per file:


| Group       | File                                                      | Ready |
| ----------- | --------------------------------------------------------- | ----- |
| nuget       | [build.release.cs](src/cangulo.cicd/build.nuget.cs)       | [x]   |
| dotnet      | [build.dotnet.cs](src/cangulo.cicd/build.dotnet.cs)       | [x]   |
| release     | [build.release.cs](src/cangulo.cicd/build.release.cs)     | [x]   |
| FileOps     | [build.release.cs](src/cangulo.cicd/build.release.cs)     | [x]   |
| Git         | [build.release.cs](src/cangulo.cicd/build.release.cs)     | [x]   |
| Changelog   | [build.release.cs](src/cangulo.cicd/build.release.cs)     | [ ]   |
| PullRequest | [build.release.cs](src/cangulo.cicd/build.pullrequest.cs) | [ ]   |

.  The execution is rule by two things:

* **Targets**: Is the CI Action want to execute. Please refer to the previous list for the available ones.
* **Target Settings**: Parameters required for each targets. For example, in order to execute any dotnet target we should fill the property `dotnetTargets` in the cicd.json

Once 

## how to use it?

This solution can be used as a GH Action. Please refer to it docs for importing:

https://www.continuousimprover.com/2020/03/reasons-for-adopting-nuke.html

# How to use this locally

```bash
# 1. Publish the dotnet project as self contained
dotnet publish ./src/cangulo.cicd/cangulo.cicd.csproj  -o ./artifacts/cangulo.cicd/ -r linux-x64 --self-contained
# 2. Example executing UT
artifacts/cangulo.cicd/cangulo.cicd ExecuteUnitTests
```