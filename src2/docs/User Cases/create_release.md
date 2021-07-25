# Create a release

1. Create a release branch from `dev`
2. Add the release tag
3. Push the branch
4. Create a PR targeting `main`
5. Common PR revisions and requirements
6. Merge the PR

In case of hotfixes, bugfixes the last tag will be used to create the branch 

```mermaid
graph TD %% Main Workflof
    %% Entities [Text Displayed]
    CREATE_BRANCH[1. Create a release branch from dev]
    ADD_TAG[2. Add the release tag]
    CREATE_PR[3. Create a PR targeting main]
    PR_REVISION_REQUIREMENTS[4. Common PR revisions and requirements]
    MERGE_PR[5. Merge the PR]
    %% Relations
    CREATE_BRANCH --> ADD_TAG --> CREATE_PR --> PR_REVISION_REQUIREMENTS -->  MERGE_PR
```