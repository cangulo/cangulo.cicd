# Develop a feature

1. [User] Create a branch from `dev`
2. [User] Create some commits
3. [User] Push the branch
4. [User] Create PR
5. [CI] Create feature nuget packages
6. [CI] Add comments of the package created
7. [User] Approve the PR
8. [User] Merge the PR
9. [CI] Create nuget packages for `dev`

# If the PR is updated
Repeat CI steps

# Commands and expected output for each step

1. [User] Create a branch from `dev`

**command:**

`git flow feature start FEATURE_NAME`

**output:**
```shell
Switched to a new branch 'feature/FEATURE_NAME'

Summary of actions:
- A new branch 'feature/FEATURE_NAME' was created, based on 'dev'
- You are now on branch 'feature/FEATURE_NAME'

Now, start committing on your feature. When done, use:

     git flow feature finish FEATURE_NAME
```

2. [User] Create some commits
3. [User] Push the branch
4. [User] Create PR

https://cli.github.com/manual/gh_pr_create

5. [CI] Create feature nuget packages
6. [CI] Add comments of the package created
7. [User] Approve the PR
8. [User] Merge the PR

The branch is deleted after the PR is merged:

**command**
`git flow feature finish FEATURE_NAME`

**output:**
```shell
Switched to branch 'dev'
Your branch is up to date with 'origin/dev'.
Already up to date.
Deleted branch feature/FEATURE_NAME (was beefd52).

Summary of actions:
- The feature branch 'feature/FEATURE_NAME' was merged into 'dev'
- Feature branch 'feature/FEATURE_NAME' has been locally deleted
- You are now on branch 'dev'
```

9. [CI] Create nuget packages for `dev`