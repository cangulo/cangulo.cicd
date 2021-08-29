using cangulo.changelog.Builders;
using System;
using System.IO;
using System.Text;

namespace cangulo.changelog.builders
{
    public interface IChangelogBuilder
    {
        void Build(string version, string[] changes, string path);
    }
    public class ChangelogBuilder : IChangelogBuilder
    {
        private readonly IChangelogVersionNotesBuilder _changelogVersionNotesBuilder;

        public ChangelogBuilder(IChangelogVersionNotesBuilder changelogVersionNotesBuilder)
        {
            _changelogVersionNotesBuilder = changelogVersionNotesBuilder ?? throw new ArgumentNullException(nameof(changelogVersionNotesBuilder));
        }

        public void Build(string version, string[] changes, string path)
        {
            var notesForThisVersion = _changelogVersionNotesBuilder.Build(version, changes);

            if (!File.Exists(path))
                AppendContent(path, notesForThisVersion);
            else
            {
                var currentContent = GetCurrentContent(path);

                var result = new StringBuilder();
                result.AppendLine(notesForThisVersion);
                result.Append(currentContent);
                
                OverwriteContent(path, result.ToString());
            }
        }

        private string GetCurrentContent(string path)
        {
            using StreamReader fileReader = new(path);
            return fileReader.ReadToEnd();
        }

        private void AppendContent(string path, string content)
        {
            using StreamWriter fileWriter = new(path, append: true);
            fileWriter.Write(content);
        }
        private void OverwriteContent(string path, string content)
        {
            using StreamWriter fileWriter = new(path, append: false);
            fileWriter.Write(content);
        }
    }
}