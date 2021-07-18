# Develop a feature

1. Create a branch from `dev`
2. Create some commits
3. Push the branch
4. Create a PR targeting `dev`
5. Common PR revisions and requirements
6. Merge the PR

```mermaid
graph TD %% Main Workflof
    %% Entities [Text Displayed]
    CREATE_BRANCH[1. Create a branch from dev]
    WORK_ON_BRANCH[2. Create some commits]
    PUSH_BRANCH[3. Push the branch]
    CREATE_PR[4. Create PR]
    PR_REVISION_REQUIREMENTS[5. Common PR revisions and requiments]
    MERGE_PR[7-Merge the PR]
    %% Relations
    CREATE_BRANCH --> WORK_ON_BRANCH --> PUSH_BRANCH --> CREATE_PR  --> PR_REVISION_REQUIREMENTS -->  MERGE_PR
```