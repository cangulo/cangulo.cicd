# Common PR updates

1. PR approved, code tested, branch updated with `target branch` ?
   1. Yes, proceed
   2. No
      1. Process PR feedback
      2. Test the development done
      3. Rebase the target branch


```mermaid
graph TD %% Main Workflof
    %% Entities [Text Displayed]
    PR_APPROVED_AND_BRANCH_UPDATED{5. PR approved and updated with target branch?}
    PUSH_NEW_COMMITS{Process the feedback or update the branch}
    PROCEED{Proceed}
    %% Relations
    PR_APPROVED_AND_BRANCH_UPDATED -- No ---> PUSH_NEW_COMMITS -- Ask for review again --> PR_APPROVED_AND_BRANCH_UPDATED
    PR_APPROVED_AND_BRANCH_UPDATED -- Yes ---> PROCEED
```