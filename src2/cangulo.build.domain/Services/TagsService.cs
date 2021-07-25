using Nuke.Common.Tooling;
using System;
using System.Linq;

namespace cangulo.build.domain
{
    public interface ITagsService
    {
        string GetLastTag();
    }

    public class TagsService : ITagsService
    {
        private readonly Tool Git;

        public TagsService(Tool git)
        {
            Git = git ?? throw new ArgumentNullException(nameof(git));
            Git("fetch --tags");
        }

        public string GetLastTag()
            => Git("tag").Select(x => x.Text).FirstOrDefault();
    }
}