# Develop a hotfix/bugfix

Please note the next steps are the same for a hotfix and a bugfix.

1. [User] Do all the workflow until finish developing the hotfix locally.
2. [User] Push the hotfix branch
3. [CI] Detect the branch is a hotfix
4. [CI] Create and push a release branch from the last tag provided in the hotfix branch
5. [User] Create a PR from the hotfix branch to the release branch
6. [User] Common PR revisions and requirements
7. [User] Merge the PR
8. [CI] Create a PR from the release branch to `dev`
9. [User] Approve the PR
10. [CI] Create a GitHub Release using GH CLI
11. [CI] Provide description for the release (Copy commits text)
12. [CI] Create artifacts
13. [CI] Push artifacts to the GitHub Release