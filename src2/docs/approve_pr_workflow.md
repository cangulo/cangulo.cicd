- [PR Model](#pr-model)
- [Main Workflow](#main-workflow)
- [Validations in Nuke](#validations-in-nuke)
- [Steps detail](#steps-detail)
  - [1. Execute all UT](#1-execute-all-ut)
  - [2. Detect Projects Changed](#2-detect-projects-changed)
  - [3. Create Prerelease Nuget Packages](#3-create-prerelease-nuget-packages)
  - [4. Push Nuget Packages](#4-push-nuget-packages)


# PR Model

| Attribute     | Value      |
| ------------- | ---------- |
| source branch | Any Branch |
| target branch | master     |

# Main Workflow

 1. Detect csproj modified
 2. Get the version provided there
 3. Get the latest version from the nuget repo
 4. [Decision] The version has been increased?
    1. No, stop
    2. Yes, create the release package

```mermaid
graph TD %% Main Workflof
    %% Entities [Text Displayed]
    EXECUTE-UT[1. Execute UT]
    TEST-RESULT{Any Test failed?}
    STOP[STOP]
    DETECT-PROJECT-CHANGED[2. Detect Projects Modified]
    CREATE-PRERELEASE-NUGETS[3. Create Prerelease Nuget Packages]
    PUSH-NUGETS[4. Push nuget packages]
    %% Relations
    EXECUTE-UT --> TEST-RESULT
    TEST-RESULT -- Yes ---> STOP
    TEST-RESULT -- No ---> DETECT-PROJECT-CHANGED
    DETECT-PROJECT-CHANGED --> CREATE-PRERELEASE-NUGETS --> PUSH-NUGETS

```
# Validations in Nuke

* This is executed in a PR

<!-- 1. List  all the solutions
2. Execute the UT for all of them
3. Check the Projects that should be packed
4. Create the nuget package for all of them -->

# Steps detail

## 1. Execute all UT
## 2. Detect Projects Changed
## 3. Create Prerelease Nuget Packages
## 4. Push Nuget Packages

