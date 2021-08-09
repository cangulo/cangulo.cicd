using cangulo.cicd.domain.Extensions;
using Nuke.Common.Tooling;

namespace cangulo.cicd.domain.Services
{
    public interface IGitService
    {
        string GetLastCommitMsg(Tool Git);
    }
    public class GitService : IGitService
    {
        public string GetLastCommitMsg(Tool Git)
            => Git($"log --format=%B -n 1", logOutput: true).ConcatenateOutputText();
    }
}
