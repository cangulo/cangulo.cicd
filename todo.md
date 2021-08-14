# TODO

- [x] Move the canculo projects to the src folder
- [x] Clean the difference between build.git and PullRequestService. Search for `PullRequestRequest` to find the duplicate code
- [ ] Fix the release body to use all the commits messages
  - [ ] 1. Get all the commits again using the service
    - [ ] Implement the IReleaseBodyBuilder
      - [ ] Fix UT, read the input from the json and the output from a txt
      - [ ] Use the IReleaseBodyBuilder to build the body
  - [ ] 2. Optionally, save the commits from the step calculate release number to a result file we will call cicd.result.json
    - [ ] Create a service to 
      - [ ] add objects to a dictionary
      - [ ] save the dictionary to a json file
      - [ ] read the json file
      - [ ] add a boolean to save the results to a json file or not
- [ ] Check what other things we can provide in the release
- [ ] Prepare what can you offer to the cangulo.changelog

https://www.continuousimprover.com/2020/03/reasons-for-adopting-nuke.html

1. Create Changelog
2. Update Changelog 
   1. Update when hotfix
   2. Update when patch
   3. Update when major