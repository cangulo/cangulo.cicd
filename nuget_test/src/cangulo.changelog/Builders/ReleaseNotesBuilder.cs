using System;
using cangulo.changelog.Builders;

namespace cangulo.changelog.builders
{
    public interface IReleaseNotesBuilder
    {
        string Build(string[] changes);
    }

    public class ReleaseNotesBuilder : IReleaseNotesBuilder
    {
        private readonly IChangesListAreaBuilder _changesListAreaBuilder;

        public ReleaseNotesBuilder(IChangesListAreaBuilder changesAreaBuilder)
        {
            _changesListAreaBuilder = changesAreaBuilder ?? throw new ArgumentNullException(nameof(changesAreaBuilder));
        }
        public string Build(string[] changes)
            => _changesListAreaBuilder.Build(changes);
    }
}
