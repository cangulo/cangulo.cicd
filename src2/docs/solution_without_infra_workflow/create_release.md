# Create a release

1. [User] Start the create release pipeline
2. [CI] Create release branch and add add the release tag
3. [CI] Push the branch
4. [CI] Create a PR from the release branch targeting `main`
5. [CI] Paste in the PR the release notes
6. [CI] Create prerelease `nuget` packages
7. [User] Common PR revisions and requirements
8. [User] Merge the PR
9. [CI] Create a GitHub Release using GH CLI
10. [CI] Provide description for the release (Copy commits text)
11. [CI] Create and push the release nuget packages / Create artifacts
12. [CI] Push artifacts to the GitHub Release

# Commands and expected output for each step

1. [User] Start the create release pipeline OR create a release branch and create a PR
2. [CI] Create release branch and add the release tag

git flow release start 0.0.2

3. [CI] Push the branch

git push

4. [CI] Create a PR from `dev` targeting `main`

https://cli.github.com/manual/gh_pr_create

gh pr create --title "Release 0.0.2" --body "Tickets created:" --base main --head release/0.0.2

6. [CI] Paste in the PR the release notes
7. [CI] Create prerelease `nuget` packages
8. [User] Common PR revisions and requirements
9. [User] Merge the PR
10. [CI] Create a GitHub Release using GH CLI
11. [CI] Provide description for the release (Copy commits text)
12. [CI] Create and push the release nuget packages / Create artifacts
13. [CI] Push artifacts to the GitHub Release