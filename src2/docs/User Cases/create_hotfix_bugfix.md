# Develop a hotfix/bugfix

Please note the next steps are the same for a hotfix and a bugfix.

1. Create a release branch from the previous release tag at `dev`. Push it.
2. Create a hotfix branch from the release branch
3. Do some commits in the hotfix branch
4. Push the branch
5. Create a PR targeting the release branch created before
6. Common PR revisions and requiments
7. Merge the PR
8. Add the new version tag to the release branch
9.  Push the release branch
10. Merge the release branch to `dev`


```mermaid
graph TD %% Main Workflof
    %% Entities [Text Displayed]
   CREATE_RELEASE_BRANCH[1. Create a release branch from the previous release tag at dev. Push it.]
   CREATE_HOTFIX_BRANCH[2. Create a hotfix branch from the release branch]
   PUSH_COMMITS[3. Do some commits in the hotfix branch]
   PUSH_BRANCH[4. Push the branch]
   CREATE_PR_TARGET_RELEASE[5. Create a PR targeting the release branch created before]
   PR_REVISION_REQUIREMENTS[6. Common PR revisions and requiments]
   MERGE_PR[7. Merge the PR]
   ADD_TAG[8. Add the new version tag to the release branch]
   PUSH_RELEASE_BRANCH[9.  Push the release branch]
   MERGE_RELEASE[10. Merge the release branch to dev]

    %% Relations
    CREATE_RELEASE_BRANCH --> CREATE_HOTFIX_BRANCH 
    --> PUSH_COMMITS --> PUSH_BRANCH 
    --> CREATE_PR_TARGET_RELEASE --> PR_REVISION_REQUIREMENTS 
    --> MERGE_PR --> ADD_TAG 
    --> PUSH_RELEASE_BRANCH --> MERGE_RELEASE    
```