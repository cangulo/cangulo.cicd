  GITHUB_BASE_REF:
  GITHUB_ACTOR:cangulo
  GITHUB_REF:refs/heads/main
  GITHUB_WORKFLOW:PR_MERGED_RELEASE_NEW_VERSION
  GITHUB_ACTION_REPOSITORY:
  GITHUB_REPOSITORY:cangulo/cangulo.cicd
  GITHUB_REPOSITORY_OWNER:cangulo
  GitHubToken:***
  PWD:/home/runner/work/cangulo.cicd/cangulo.cicd
  GITHUB_SHA:5a3dc210f82a1c81dab5255854b3657631f393f2
  GITHUB_PATH:/home/runner/work/_temp/_runner_file_commands/add_path_4c62d19b-d62c-4afb-abd8-131925480787
  GITHUB_RETENTION_DAYS:90
  GITHUB_EVENT_NAME:push
  GITHUB_RUN_ID:1085850004
  GITHUB_ACTION_REF:
  GITHUB_GRAPHQL_URL:https://api.github.com/graphql
  GITHUB_ENV:/home/runner/work/_temp/_runner_file_commands/set_env_4c62d19b-d62c-4afb-abd8-131925480787
  GITHUB_ACTION:__run
  GITHUB_JOB:ubuntu-latest
  GITHUB_HEAD_REF:
  GITHUB_WORKSPACE:/home/runner/work/cangulo.cicd/cangulo.cicd


Logger.Info($"{JsonSerializer.Serialize(GitHubActions, SerializerContants.SERIALIZER_OPTIONS)}");

{
"Home": "/home/runner",
"GitHubWorkflow": "PR_MERGED_RELEASE_NEW_VERSION",
"GitHubAction": "__run",
"GitHubActor": "cangulo",
"GitHubRepository": "cangulo/cangulo.cicd",
"GitHubRepositoryOwner": "cangulo",
"GitHubEventName": "push",
"GitHubEventPath": "/home/runner/work/_temp/_github_workflow/event.json",
"GitHubWorkspace": "/home/runner/work/cangulo.cicd/cangulo.cicd",
"GitHubSha": "7a76c56cb440ffbc37d171d51cf56ef10fbdb3f9",
"GitHubRef": "refs/heads/main",
"GitHubHeadRef": "",
"GitHubBaseRef": "",
"GitHubRunNumber": 19,
"GitHubRunId": 1086158973,
"GitHubServerUrl": "https://github.com",
"GitHubJob": "ubuntu-latest"
}