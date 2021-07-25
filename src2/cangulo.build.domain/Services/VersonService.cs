using cangulo.build.abstractions.Models;

namespace cangulo.build.domain
{
    public interface IVersionService
    {
        ProgramVersion ParseVersionFromTag(string tag);
    }

    public class VersionService : IVersionService
    {
        public ProgramVersion ParseVersionFromTag(string tag)
            => new ProgramVersion
            {
                Major = int.Parse(tag.Split(".")[0]),
                Minor = int.Parse(tag.Split(".")[1]),
                Patch = int.Parse(tag.Split(".")[2])
            };
    }
}