using System.Threading.Tasks;

namespace cangulo.cicd.domain.Services
{
    public interface IChangeLogService
    {
        Task AddRelease(string release, string[] changes);
    }
    public class ChangeLogService : IChangeLogService
    {
        public const string CHANGELOGFILENAME = "CHANGELOG.md";
        public Task AddRelease(string release, string[] changes)
        {
            throw new System.NotImplementedException();
        }
    }
}
