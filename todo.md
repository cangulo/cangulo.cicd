# TODO

- [x] Move the canculo projects to the src folder
- [x] Clean the difference between build.git and PullRequestService. Search for `PullRequestRequest` to find the duplicate code
- [x] Fix the release body to use all the commits messages
  - [x] 1. Get all the commits again using the service
  - [x] 2. Implement the IReleaseBodyBuilder
    - [x] Fix UT, read the input from the json and the output from a txt
    - [x] Use the IReleaseBodyBuilder to build the body
- [ ] **Create Result BAG** 
  - [ ] Save the commits from the step _calculate release number_ to a result file we will call cicd.resultbag.json, the class will be ResultBag
  - [ ] Create a service to 
    - [ ] add objects to a dictionary
    - [ ] save the dictionary to a json file
    - [ ] read the json file
    - [ ] add a boolean to save the results to a json file or not
- [ ] **Create cangulo.changelog** 
  - [ ] 1. Create a service to create a changelog for a specific version providing a list of changes
  - [ ] 2. Extend functionality to classific commits depending on the convention commits
  - [ ] 3. Extend functionality to only list the PRs names and add links
  - [ ] **Extend the changelog to update the changelog.md file**
    - [ ] Keep only 5 versions
    - [ ] Create a md file with the changes every release in a changelog folder
    - [ ] Create a index for all the files
- [ ] Check what other things we can provide in the release
- [ ] Prepare what can you offer to the cangulo.changelog

https://www.continuousimprover.com/2020/03/reasons-for-adopting-nuke.html

1. Create Changelog
2. Update Changelog 
   1. Update when hotfix
   2. Update when patch
   3. Update when major