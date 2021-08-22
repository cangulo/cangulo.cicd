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


| Group   | File                                                  |
| ------- | ----------------------------------------------------- |
| dotnet  | [build.dotnet.cs](src/cangulo.cicd/build.dotnet.cs)   |
| release | [build.release.cs](src/cangulo.cicd/build.release.cs) |

.  The execution is rule by two things:

* **Targets**: Is the CI Action want to execute. Please refer to the previous list for the available ones.
* **Target Settings**: Parameters required for each targets. For example, in order to execute any dotnet target we should fill the property `dotnetTargets` in the cicd.json

Once 

## how to use it?

This solution can be used as a GH Action. Please refer to it docs for importing:


# TODO

- [ ] **Group targets by groups** 
  - [ ] Move compress directory to fileops
- [ ] **Clean the release process** 
  - [ ] Read a release change property from the resultbag
- [ ] **Create PR validation**
  - [ ] UT pass
  - [ ] Convention commit provided
- [ ] **Find a way to define all available targets in the .md file**


https://www.continuousimprover.com/2020/03/reasons-for-adopting-nuke.html

1. Create Changelog
2. Update Changelog 
   1. Update when hotfix
   2. Update when patch
   3. Update when major


# How to use this locally

```bash
# cangulo.cicd
dotnet publish ./src/cangulo.cicd/cangulo.cicd.csproj  -o ./artifacts/cangulo.cicd/ -r linux-x64 --self-contained

nuke ExecuteUnitTests
nuke Publish
artifacts/cangulo.cicd/cangulo.cicd ExecuteUnitTests
rm -rf ../cangulo.changelog/cangulo.cicd
mv artifacts/** ../cangulo.changelog/cangulo.cicd

# cangulo.changelog
cangulo.cicd/cangulo.cicd ExecuteUnitTests  
```