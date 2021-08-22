# cangulo.changelog

## Goals

Main Goal:
* Offer a solution for all CICD operations as execute tests, create release, update changelog , along different repositories using NUKE
* Make it customizable  by providing settings in a cicd.json file
* List the result in a json format so can be used in any pipeline

## How does this work?

1. Provide settings in the cicd.json file
2. If result should be output 


# TODO

- [x] Move the cangulo projects to the src folder
- [x] Clean the difference between build.git and PullRequestService. Search for `PullRequestRequest` to find the duplicate code
- [x] Fix the release body to use all the commits messages
  - [x] 1. Get all the commits again using the service
  - [x] 2. Implement the IReleaseBodyBuilder
    - [x] Fix UT, read the input from the json and the output from a txt
    - [x] Use the IReleaseBodyBuilder to build the body
- [x] **Create Result BAG** 
  - [x] Save the commits from the step _calculate release number_ to a result file we will call cicd.resultbag.json, the class will be ResultBag
  - [x] Create a service to 
    - [x] add objects to a dictionary
    - [x] save the dictionary to a json file
    - [x] read the json file

https://www.continuousimprover.com/2020/03/reasons-for-adopting-nuke.html

1. Create Changelog
2. Update Changelog 
   1. Update when hotfix
   2. Update when patch
   3. Update when major


# How to use this locally

```bash
# cangulo.cicd
nuke ExecuteUnitTests
nuke Publish
artifacts/cangulo.cicd/cangulo.cicd ExecuteUnitTests
rm -rf ../cangulo.changelog/cangulo.cicd
mv artifacts/** ../cangulo.changelog/cangulo.cicd

# cangulo.changelog
cangulo.cicd/cangulo.cicd ExecuteUnitTests  
```